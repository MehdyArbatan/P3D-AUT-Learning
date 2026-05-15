using System.Collections.Generic;
using System.Collections.Specialized;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using Autodesk.ProcessPower.PnP3dObjects;
using Autodesk.ProcessPower.PlantInstance;
using Autodesk.ProcessPower.DataLinks;
using Autodesk.ProcessPower.ProjectManager;
using Autodesk.AutoCAD.Windows;
using System.Windows.Forms.Integration;

namespace Stage3_Basics
{
    public class InitiaLForm
    {
        static PaletteSet SVSPalette = null;
        //We make it null so everytime you request the form, it would not create a new palette for it,
        //by creating the IF structure in the method below.
        //We also used static to keep the palette alive even after the method is done executing, since if we do not use static,
        //the palette would be created everytime you call the method.

        [CommandMethod("INITFORM")]

        public void ShowForm()
        {
            if (SVSPalette == null)
            {
                SVSPalette = new PaletteSet("SVS Palette",new Guid("A1B2C3D4-E5F6-4789-A0B1-C2D3E4F50000"));
                //here we create a new palette and give it a name and a guid. The guid is used to identify the palette and should be unique.
                //The reason behind this is that AutoCAD only knows it special palette for showing windows forms.
                //So we need to create a new palette for our WPF form.

                SVSPalette.MinimumSize = new System.Drawing.Size(350, 100);
                //Trying to introduce some discipline into the palette.

                InitialForm MyUI = new InitialForm();
                //Initilizing linking the WPF you created to the palette you just created.

                ElementHost WPFHost = new ElementHost();
                //We are creaing this ElementHost to host the WPF form in the palette, since if
                //we directly link the WPF form to the palette, it would not work, we need to use this ElementHost as a bridge.
                //It is like the WPF should go inside this adaptor (ElementHost) and then the adaptor goes to the palette.

                WPFHost.AutoSize = true;
                WPFHost.Dock = System.Windows.Forms.DockStyle.Fill;
                WPFHost.Child = MyUI;
                //And here we define all the visual properties of the ElementHost and link the WPF form to it.

                SVSPalette.Add("SVS Palette", WPFHost);
            }

            SVSPalette.Visible = true;
        }

    }
}
