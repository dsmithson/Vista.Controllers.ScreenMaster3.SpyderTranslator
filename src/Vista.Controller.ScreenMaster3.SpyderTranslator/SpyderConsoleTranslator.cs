using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vista.Controllers.ScreenMaster3.MessageQueue.Client;
using Vista.Controllers.ScreenMaster3.MessageQueue.Messages;
using Spyder.Console;
using Spyder.Console.Packets;
using Spyder.Console.Modules;

namespace Vista.Controller.ScreenMaster3.SpyderTranslator
{
    /// <summary>
    /// Translates commands and updates between a Spyder 200/300/X20/X80 console and a ScreenMaster3 message queue
    /// </summary>
    public class SpyderConsoleTranslator
    {
        private readonly ConsoleSimClient spyderConsole;
        RabbitMqClient screenMasterConsole;
        ButtonTranslationMap buttonMap;

        public bool IsRunning { get; set; }

        public SpyderConsoleTranslator()
        {
            spyderConsole = new ConsoleSimClient();
        }


        public async Task<bool> StartupAsync(SpyderConsoleTranslatorConfig config)
        {
            await ShutdownAsync();
            IsRunning = true;

            buttonMap = config.ButtonTranslationMap;
            if (buttonMap == null)
                throw new NullReferenceException($"{nameof(config.ButtonTranslationMap)} supplied in config is required but was null");

            //Start ScreenMaster console
            screenMasterConsole = new RabbitMqClient(config.RabbitMQHost, config.RabbitMQUser, config.RabbitMQPass, config.RAbbitMQVirtualHost);
            screenMasterConsole.KeyAction += ScreenMasterConsole_KeyAction;
            screenMasterConsole.JoystickAction += ScreenMasterConsole_JoystickAction;
            screenMasterConsole.TBarAction += ScreenMasterConsole_TBarAction;
            screenMasterConsole.RotaryAction += ScreenMasterConsole_RotaryAction;
            if(!await screenMasterConsole.Startup())
            {
                Console.WriteLine("Failed to start ScreenMaster console.  Shutting down...");
                await ShutdownAsync();
                return false;
            }

            //Start spyder console
            spyderConsole.Refresh += SpyderConsole_Refresh;
            spyderConsole.Startup(IPAddress.Parse(config.SpyderServerIP));

            //Wait for console connect (fagile)
            await Task.Delay(10000);

            //Set Initial Console Segments
            await SetSpyderConsoleSegments(buttonMap.StartupSegmentConfigs).ConfigureAwait(false);

            //Press and release custom buttons as defined in startup
            foreach(var button in buttonMap.KeyMappings.Values.Where(key => key.PressAtStartup))
            {
                await PressAndRelease(button).ConfigureAwait(false);
            }

            return true;
        }

        public async Task ShutdownAsync()
        {
            IsRunning = false;

            if (spyderConsole != null)
            {
                spyderConsole.Refresh -= SpyderConsole_Refresh;
                spyderConsole.Shutdown();
            }

            if (screenMasterConsole != null)
            {
                screenMasterConsole.KeyAction += ScreenMasterConsole_KeyAction;
                screenMasterConsole.JoystickAction += ScreenMasterConsole_JoystickAction;
                screenMasterConsole.TBarAction += ScreenMasterConsole_TBarAction;
                screenMasterConsole.RotaryAction -= ScreenMasterConsole_RotaryAction;
                await screenMasterConsole.ShutdownAsync();
                screenMasterConsole = null;
            }
        }

        public ConsoleSimClient GetSpyderSimClient()
        {
            return spyderConsole;
        }

        private void ScreenMasterConsole_RotaryAction(object sender, RotaryActionEventArgs e)
        {
            if (IsRunning)
                spyderConsole.RotaryAction(e.RotaryIndex, e.RotaryOffset);
        }

        private void ScreenMasterConsole_TBarAction(object sender, TBarActionEventArgs e)
        {
            if (IsRunning)
                spyderConsole.TBarAction(e.TBarPosition);
        }

        private void ScreenMasterConsole_JoystickAction(object sender, JoystickActionEventArgs e)
        {
            if(IsRunning)
                spyderConsole.JoystickAction(e.X, e.Y, e.Z);
        }

        private async void ScreenMasterConsole_KeyAction(object sender, KeyActionEventArgs e)
        {
            //Look up button reference
            var mapping = buttonMap.GetConfigForButton(e.KeyIndex);
            if(mapping != null)
            {
                if(mapping.CustomKey != null)
                {
                    if (e.IsPressed)
                    {
                        //Light pressed button
                        screenMasterConsole.SetLamps(true, e.KeyIndex);

                        //Clear light on other buttons in same group
                        var keyConfig = buttonMap.GetConfigForButton(e.KeyIndex);
                        if (keyConfig != null && !string.IsNullOrEmpty(keyConfig.CustomGroup))
                        {
                            var otherGroupKeys = buttonMap.GetKeysInGroup(keyConfig.CustomGroup)
                                .Except(new int[] { e.KeyIndex })
                                .ToArray();

                            if(otherGroupKeys.Length > 0)
                            {
                                screenMasterConsole.SetLamps(false, otherGroupKeys);
                            }
                        }

                        //Process segment changes based on selection
                        await SetSpyderConsoleSegments(buttonMap.GetSegmentConfigsForCustomKey(keyConfig.CustomKey)).ConfigureAwait(false);
                    }
                }
                else
                {
                    //Send direct key action to Spyder
                    spyderConsole.ButtonAction(mapping.SegmentID, mapping.SpyderButtonIndex, e.IsPressed);
                }
            }
        }

        private async Task SetSpyderConsoleSegments(IEnumerable<SegmentConfig> segments)
        {
            if (segments == null || !segments.Any())
                return;

            var layoutButton = new KeyConfig() { SegmentID = 18, SpyderButtonIndex = 6 };
            var editButton = new KeyConfig() { SegmentID = 18, SpyderButtonIndex = 14 };

            //Press and release buttons to put keyboard into edit mode
            await PressAndRelease(layoutButton).ConfigureAwait(false);
            await PressAndRelease(editButton).ConfigureAwait(false);

            //Press segment buttons to select mode
            foreach(SegmentConfig segment in segments)
            {
                //Segment Type enum maps to the button index on our segment we are setting
                await PressAndRelease(segment.SegmentID, (int)segment.SegmentType).ConfigureAwait(false);
                await PressAndRelease(segment.SegmentID, 8 + segment.Index).ConfigureAwait(false);
            }

            //Press and release button to exit edit mode
            await PressAndRelease(editButton).ConfigureAwait(false);
        }

        private async Task PressAndRelease(KeyConfig spyderKey)
        {
            await PressAndRelease(spyderKey.SegmentID, spyderKey.SpyderButtonIndex).ConfigureAwait(false);
        }

        private async Task PressAndRelease(int segmentID, int buttonIndex)
        {
            if (IsRunning)
            {
                spyderConsole.ButtonAction(segmentID, buttonIndex, true);
                await Task.Delay(20).ConfigureAwait(false);
                spyderConsole.ButtonAction(segmentID, buttonIndex, false);
                await Task.Delay(20).ConfigureAwait(false);
            }
        }

        private void SpyderConsole_Refresh(IPRefreshPacket pkt)
        {
            var newSegments = pkt.GetPortRefresh();

            List<int> keysOn = new List<int>();
            List<int> keysOff = new List<int>();
            List<QuickKeyButton> quickKeys = new List<QuickKeyButton>();
            foreach (var newSegment in newSegments)
            {
                int segmentID = newSegment.Key;
                var segment = newSegment.Value;

                var spyderKeys = segment.ColorButtons;
                var sm3Keys = buttonMap.GetKeysFromSegmentID(segmentID);
                var mappings = from spyderKey in spyderKeys
                               join sm3Key in sm3Keys on spyderKey.ControlID equals sm3Key.Value.SpyderButtonIndex
                               select new
                               {
                                   Sm3Index = sm3Key.Key,
                                   Mapping = sm3Key.Value,
                                   SpyderKey = spyderKey,
                                   spyderKey.Text,
                                   QuickKeyIndex = KeyConfig.GetQuickKeyIndex(sm3Key.Key)
                               };

                var lampMappings = from mapping in mappings
                                   where mapping.QuickKeyIndex == -1
                                   select new
                                   {
                                       Index = mapping.Sm3Index,
                                       LampOn = ShouldLampBeOn(mapping.SpyderKey)
                                   };

                var quickKeyMappings = from mapping in mappings
                                       where mapping.QuickKeyIndex != -1
                                       select new QuickKeyButton()
                                       {
                                           Index = mapping.QuickKeyIndex,
                                           Text = GetQuickKeyText(segment, mapping.SpyderKey),
                                           Color = GetQuickKeyColor(mapping.SpyderKey)
                                       };

                keysOn.AddRange(lampMappings.Where(key => key.LampOn).Select(key => key.Index));
                keysOff.AddRange(lampMappings.Where(key => !key.LampOn).Select(key => key.Index));
                quickKeys.AddRange(quickKeyMappings);
            }

            //Send lamp updates to SM3
            screenMasterConsole.SetLamps(true, keysOn.ToArray());
            screenMasterConsole.SetLamps(false, keysOff.ToArray());
            screenMasterConsole.SetQuickKeys(quickKeys.ToArray());
        }

        private bool ShouldLampBeOn(ColorButton button)
        {
            //Color index 0 is off on the Spyder's RGB buttons.  
            //If a button is dim or off, then don't light the SM3 button
            if (button.Dim || button.Color == (int)PaletteColors.PropertyUnselected || button.Color == (int)PaletteColors.PropertyUnselected)
                return false;

            return true;
        }

        private string GetQuickKeyText(Segment segment, ColorButton button)
        {
            const int charsPerButtonPerLine = 5;
            const int buttonsWide = 8;
            const int row2StartIndex = buttonsWide * charsPerButtonPerLine;

            string displayText = segment.GetDisplayText();
            if (string.IsNullOrEmpty(displayText))
                return button.Text;

            string line1 = displayText.Substring(button.ControlID * charsPerButtonPerLine, charsPerButtonPerLine);
            string line2 = displayText.Substring(button.ControlID * charsPerButtonPerLine + row2StartIndex, charsPerButtonPerLine);
            return string.Format($"{line1} {line2}");
        }

        private QuickKeyColor GetQuickKeyColor(ColorButton button)
        {
            if (button.Color == (int)PaletteColors.Off)
                return QuickKeyColor.Off;

            else if (button.Dim)
                return QuickKeyColor.Green;

            return QuickKeyColor.Red;
        }
    }
}
