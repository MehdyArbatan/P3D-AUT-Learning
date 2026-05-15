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

namespace Stage2_Basics
{
    public class BatchOpt
    {
        [CommandMethod("BatchPipeCounter")]

        public void PipeCounter()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            TypedValue[] tvs = new TypedValue[1];
            //TypedValue is the object associated with AutoCad search rules
            //and you are saying that you need an array of it with only 1 member

            tvs[0] = new TypedValue((int)DxfCode.Start, "*Pipe");
            //now you are creating the first rule with TypedValue and it needs a pair of data:
            //The property to check and the value to check for.
            //In this case, you are saying that you want to check the "Start" property of the object
            //and see if it is equal to "*Pipe", which is the name of the pipe object in AutoCAD

            SelectionFilter PipeFilter = new SelectionFilter(tvs);
            //now you are creating a SelectionFilter object with the array of TypedValue as the argument

            PromptSelectionResult SelRes = ed.SelectAll(PipeFilter);
            //now you are using the Editor object to select all objects in the drawing that match the filter criteria
            //the result of the selection is stored in a PromptSelectionResult object which contains information
            //about the selection, including the status and the selected objects.

            if (SelRes.Status == PromptStatus.OK)
            {
                ed.WriteMessage("\nFound " + SelRes.Value.Count + " pipes in the model");
                //remember that PrompSelectionResult has different info about the selection.
                //You are now using the "Value" property of the PromptSelectionResult to get the collection of selected objects,
                //and then using the "Count" property of that collection (which is a SelectionSet) to get the number of pipes
                //found in the model.

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    PlantProject currentProject = PlantApplication.CurrentProject;
                    Project pipingProj = currentProject.ProjectParts["Piping"];
                    DataLinksManager dlm = pipingProj.DataLinksManager;

                    StringCollection labels = new StringCollection();
                    labels.Add("Size");
                    labels.Add("Spec");
                    labels.Add("LineNumberTag");

                    using (StreamWriter ExportWrite = new StreamWriter("D:\\PipeData.txt"))
                    {
                        foreach (SelectedObject selObj in SelRes.Value)
                        //SelectedObject is a class that represents an object that has been selected in AutoCAD in the SelectionSet.
                        //It contains information about the selected object, such as its ObjectId and its properties.
                        {
                            ObjectId objId = selObj.ObjectId;
                            Entity PipeEntity = (Entity)tr.GetObject(objId, OpenMode.ForRead);
                            //we got the ObjectID from the PromptSelection. You can querry any properties from the DLM by just having
                            //the ObjectID. We need the PipeEntity to change its layer, color, or move its geometry.
                            //Remeber, there are DWG data and P3D data. The DWG data is what you see in AutoCAD,
                            //which is the geometry and properties of the objects and P3D data is the data that is stored in the datalink tables,
                            //which is the properties that you see in P3D.

                            int rowID = dlm.FindAcPpRowId(objId);
                            if (rowID > 0)
                            {
                                var results = dlm.GetProperties(rowID, labels, true);
                                ed.WriteMessage($"\n {objId}: Line number:{results[2]} - Spec:{results[1]} - Size:{results[0]}");
                                ExportWrite.WriteLine($"{objId}: Line number:{results[2]} - Spec:{results[1]} - Size:{results[0]}");
                            }
                        }
                    }
                }
            }
            else
            {
                ed.WriteMessage("\nNo pipes found in the model");
            }
        }
    }
}
