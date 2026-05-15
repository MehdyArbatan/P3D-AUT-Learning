using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Stage0_Basics
{
    public class Commands2
    {
        [CommandMethod("InputMove")]

        public void MoveByInputMethod()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityResult SelectedEntity = ed.GetEntity("\nSelect an object you want to be moved: ");

            if (SelectedEntity.Status != PromptStatus.OK)
                return;

            PromptDoubleOptions opt = new PromptDoubleOptions("\nEnter the distance to move: "); 
            // PromptDoubleOptions is a class that defines the options for prompting the user to enter a double value.
            // It allows you to specify various settings for the prompt, such as the message displayed to the user, the default value, and constraints on the input.

            opt.AllowNegative = true;
            opt.AllowZero = false;

            PromptDoubleResult DistRes = ed.GetDouble(opt);
            // PromptDoubleResult is a class that represents the result of prompting the user to enter a double value.
            // It contains properties such as Status, Value, and StringResult.

            if (DistRes.Status != PromptStatus.OK)
                return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(SelectedEntity.ObjectId, OpenMode.ForWrite) as Entity;
                
                if (ent == null)
                    return;

                Vector3d MoveVector = new Vector3d(DistRes.Value, 0, 0);
                ent.TransformBy(Matrix3d.Displacement(MoveVector));

                tr.Commit();
            }

        }
    }
}
