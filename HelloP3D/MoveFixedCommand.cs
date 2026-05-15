using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Stage0_Basics
{
    public class Commands
    {
        [CommandMethod("MoveFixed")]
        public void MoveFixedMethod()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // Get the current active document in AutoCAD

            Database db = doc.Database;
            // Get the database associated with the current document

            Editor ed = doc.Editor;
            // Create the editor object to interact with the user

            PromptEntityResult result = ed.GetEntity("\nSelect an object to be amazed: ");
            // Stops everthing and waits for the user to select an entity in the AutoCAD drawing.
            // The prompt message is displayed to guide the user.

            if (result.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction()) 
                    // Create a transaction to safely access and modify the database.
                    // Transaction is a mechanism to ensure that a series of operations on the database are
                    // treated as a single unit of work. If any operation fails,
                    // the transaction can be rolled back to maintain data integrity.
                {
                    Entity ent = tr.GetObject(result.ObjectId, OpenMode.ForWrite) as Entity;
                    // Attempt to cast the selected object to an Entity
                    // ent: A generic drawable AutoCAD object.

                    if (ent != null)
                    {
                        ed.WriteMessage($"\nYou selected a {ent.GetType().Name} with handle {ent.Handle}.");

                        Vector3d MoveVector = new Vector3d(1000, 0, 0);
                        // Move 1000 units in the X direction
                        // Vector3d is a structure that represents a vector in 3D space, defined by its X, Y, and Z components.

                        ent.TransformBy(Matrix3d.Displacement(MoveVector));
                        // TransformBy applies a transformation to the entity.
                        // In this case, we are applying a displacement transformation that moves the entity by the specified vector.

                        tr.Commit();
                        // Commit the transaction to save changes to the database.
                        // If this line is not reached, the transaction will be rolled back automatically when it goes out of scope.
                    }
                    else
                    {
                        ed.WriteMessage("\nThe selected object is not an entity.");
                    }
                }
            }

        }
    }
}