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

namespace Stage1_Basics
{
    public class DataGather
    {
        [CommandMethod("GETPLANTDATA")]

        public void GatherData()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PlantProject currentProject = PlantApplication.CurrentProject;
            // Get the current project

            Project pipingProj = currentProject.ProjectParts["Piping"];
            // Get the piping project.
            // ProjectParts is a dictionary of all the projects in the current project,with the project name as the key.
            // Different terms for ProjectParts are Electrical, Instrumentation, HVAC, etc.

            DataLinksManager dlm = pipingProj.DataLinksManager;
            // Get the DataLinksManager for the piping project.
            // DataLinksManager is the main class for working with datalinks.

            PromptEntityOptions SelectedEntity = new PromptEntityOptions("\nSelect an object to gather data from: ");
            SelectedEntity.SetRejectMessage("\nOnly 3D objects are allowed.");
            SelectedEntity.AddAllowedClass(typeof(Entity), false);
            //PromptEntityOptions is a class that defines the options for prompting the user to select an entity in the AutoCAD drawing.
            //It allows you to specify various settings for the prompt, such as the message displayed to the user,
            //filters for allowed entity types, and rejection messages for invalid selections.

            PromptEntityResult ResultedEntity = ed.GetEntity(SelectedEntity);
            //difference between PromptEntityOptions and PromptEntityResult:
            //PromptEntityOptions is used to define the options for prompting the user to select an entity,
            //while PromptEntityResult is used to capture the result of that prompt,
            //including the status of the selection and the details of the selected entity.
            //The reason that we yse Options first and then Result is to define filters for the user to follow.

            if (ResultedEntity.Status != PromptStatus.OK) return;

            int rowID = dlm.FindAcPpRowId(ResultedEntity.ObjectId);
            //FindAcPpRowId is a method of the DataLinksManager class that takes an ObjectId as input
            //and returns the corresponding row ID from the datalink table.
            //difference between ObjectId and RowID: ObjectID is a unique identifier for an entity in the AutoCAD drawing,
            //while RowID is a unique identifier for a row in the datalink table associated with that entity.

            if (rowID > 0)
            {
                List<KeyValuePair<string,string>> AllProps = dlm.GetAllProperties(rowID,true);
                //This is a List of Lists that contains all the P3D properties of the selected object.
                //The first string is the property name and the second string is the property value.

                /*foreach (var prop in AllProps)
                {
                   ed.WriteMessage($"\nProperty Name: {prop.Key}, Property Value: {prop.Value}");
                }*/

                StringCollection labels = new StringCollection();
                labels.Add("Size");
                labels.Add("Spec");
                labels.Add("LineNumberTag");
                //The reason we do this is because GetProperties can only take a StringCollection as an argument,
                //so we need to convert our desired strings into a "StringCollection".

                var results = dlm.GetProperties(rowID, labels, true);
                //GetProperties is a method of the DataLinksManager class that takes a row ID, a StringCollection of property names, and a boolean indicating whether to include inherited properties.

                ed.WriteMessage($"\n---Selected elements properties---");
                ed.WriteMessage($"\nSize: {results[0]}");
                ed.WriteMessage($"\nSpec: {results[1]}");
                ed.WriteMessage($"\nLine Number: {results[2]}");
                //0, 1 & 2 are the exact order of properties you asked when you created the "labels" StringCollection.

            }
            else return;

        }
    }
}
