using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System.Windows.Forms.Integration;

namespace ReviewProject
{
    public class ReviewProject
    {
        static PaletteSet RPPalette = null;

        [CommandMethod("RPP")]

        public void ShowForm()
        {
            if (RPPalette == null)
            {
                //1. Create a new PaletteSet
                RPPalette = new PaletteSet("Review Project Palette", new Guid("B1C2D3E4-F5A6-4789-B0C1-D2E3F4G50000"));
                RPPalette.MinimumSize = new System.Drawing.Size(400, 200);

                //2. Create an instance of the WPF you customly created.
                MainForm MyUI = new MainForm();

                //3. Create an ElementHost which is practically and adapter.
                ElementHost WPFHost = new ElementHost();
                WPFHost.AutoSize = true;
                WPFHost.Dock = System.Windows.Forms.DockStyle.Fill;

                //4. Put your WPF inside the adapter.
                WPFHost.Child = MyUI;

                //5. Put the adapter inside the PaletteSet.
                RPPalette.Add("Review Project", WPFHost);
            }
            //6. Show the PaletteSet
            RPPalette.Visible = true;
        }
    }
}
