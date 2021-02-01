using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace Spyder.Console.Controls.Bindable
{
    public class BindableControlSegment : BindableSegment
    {
        private List<BindableControlBase> controls = new List<BindableControlBase>();
        public List<BindableControlBase> Controls
        {
            get { return controls; }
        }

        public BindableControlSegment() : base()
        {
        }

        public void AddControl(int buttonIndex, BindableControlBase control)
        {
            RemoveControl(buttonIndex);
            if (control != null)
            {
                control.PropertyChanged += new PropertyChangedEventHandler(control_PropertyChanged);
                controls.Add(control);
            }
        }

        /// <summary>
        /// Reuses existing object instances if the desired control type is already present
        /// </summary>
        public BindableControlBase GetOrCreate(int buttonIndex, Type controlType)
        {
            BindableControlBase control = GetControl(buttonIndex);
            if (control != null && controlType.Equals(control.GetType()))
                return control;

            //Create new control
            ConstructorInfo constructor = controlType.GetConstructor(new Type[0]);
            if (constructor != null)
            {
                control = constructor.Invoke(null) as BindableControlBase;
                if (control != null)
                    AddControl(buttonIndex, control);
            }
            return control;
        }

        public BindableControlBase GetControl(int buttonIndex)
        {
            return controls.FirstOrDefault(control => control.ButtonIndex == buttonIndex);
        }

        public void RemoveControl(int buttonIndex)
        {
            BindableControlBase control = GetControl(buttonIndex);
            if (control != null)
            {
                control.PropertyChanged -= new PropertyChangedEventHandler(control_PropertyChanged);
                controls.Remove(control);
            }
        }

        public void ClearControls()
        {
            while (controls.Count > 0)
                RemoveControl(controls[0].ButtonIndex);
        }

        public void RenderControls()
        {
            foreach (BindableControlBase control in controls)
                control.RenderControl(this);
        }

        void control_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
