using System;
using System.Windows.Media;
using System.Collections.Generic;

namespace Spyder.Console
{
	[Serializable]
	public class ColorPalette
	{
		List<Color> colors = new List<Color>();

		private int dimFactor = 60;
		public int DimFactor
		{
			get { return dimFactor; }
			set { dimFactor = value; }
		}

		public ColorPalette()
		{
			dimFactor = 60;
			colors.Insert((int)PaletteColors.Off, System.Windows.Media.Colors.Black);                         // off
			colors.Insert((int)PaletteColors.On, Color.FromRgb(0, 0, 73));             // on 
			colors.Insert((int)PaletteColors.Program, Color.FromRgb(91, 0, 0));        // program
			colors.Insert((int)PaletteColors.Preview, Color.FromRgb(120, 42, 0));      // preview
			colors.Insert((int)PaletteColors.PropertyLayer, Color.FromRgb(97, 0, 97)); // propLayer 
			colors.Insert((int)PaletteColors.Config, Color.FromRgb(0, 54, 0));         // config
			colors.Insert((int)PaletteColors.CmdKeys, Color.FromRgb(119, 16, 0));      // command keys
			colors.Insert((int)PaletteColors.Layout, Color.FromRgb(0, 128, 0));            // layout
			colors.Insert((int)PaletteColors.Effect, Color.FromRgb(154, 205, 50));         // effect
			colors.Insert((int)PaletteColors.DDR, Color.FromRgb(178, 34, 34));             // DDR
			colors.Insert((int)PaletteColors.Treat, Color.FromRgb(93, 59, 0));         // Treat
			colors.Insert((int)PaletteColors.Source, Color.FromRgb(0, 0, 92));         // Source
			colors.Insert((int)PaletteColors.FuncKey, Color.FromRgb(0, 90, 90));       // QuickKey
			colors.Insert((int)PaletteColors.LayerSC, Color.FromRgb(0, 0, 92));        // LayerSC
			colors.Insert((int)PaletteColors.PropertyUnselected, Color.FromRgb(50, 50, 50));       // MixerSC
			colors.Insert((int)PaletteColors.LayerOff, Color.FromRgb(0, 55, 0));       // SourceSC
			colors.Insert((int)PaletteColors.Still, System.Windows.Media.Colors.Gray);
		}

		public List<Color> Colors { get { return colors; } set { colors = value; } }


		public Color this[int index]
		{
			get
			{
				if (index >= colors.Count)
					return System.Windows.Media.Colors.Black;

				return colors[index];
			}
			set
			{
				if (index >= colors.Count)
					return;

				colors[index] = value;
			}
		}
		public Color this[PaletteColors color]
		{
			get
			{
				if ((int)color >= colors.Count)
					return System.Windows.Media.Colors.Black;

				return colors[(int)color];
			}
			set
			{
				if ((int)color >= colors.Count)
					return;

				colors[(int)color] = value;
			}
		}
		public Color DimColor(Color color)
		{
			float factor = (float)dimFactor / (float)255;
			byte r, g, b;
			r = (byte)((float)color.R * factor);
			g = (byte)((float)color.G * factor);
			b = (byte)((float)color.B * factor);

			return Color.FromArgb(255, r, g, b);
		}
	}
}
