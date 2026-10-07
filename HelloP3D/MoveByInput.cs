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
            /* PromptDoubleOptions is a class that defines the options for prompting the user to enter a double value.
            It allows you to specify various settings for the prompt, such as the message displayed to the user, the default value, and constraints on the input.
            Remember, by defining this, you are not showing the prompt to the user yet. You are just defining what to ask the User
            and what kind of restrictions should be applied to what user can actually enter*/

            opt.AllowNegative = true;
            opt.AllowZero = false;
            opt.DefaultValue = 1000.0;

            PromptDoubleResult DistRes = ed.GetDouble(opt);
            /* Now you are showing the message you already defined by PromptDouble Options to the user.
            You need to broaden your horizion. Even in this level, the only variable is not just what user enters.
            For example, the value he enters, whether he presses enter or esc or other scenarios.
            This is the main reasoning behind all of these class and object definitions*/

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
