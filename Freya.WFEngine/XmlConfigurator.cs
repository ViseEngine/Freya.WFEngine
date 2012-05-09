using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    public class XmlConfigurator
    {
        private readonly IList<XmlElement> configurations = new List<XmlElement>(); 

        public void Configure<TItem>(Workflow<TItem> workflow) {
            foreach (var workflowConfiguration in configurations) {
                this.LoadActivityDefinitions(workflowConfiguration.SelectSingleNode(SR.ActivitiesElementName));
                this.LoadGuardDefinitions(workflowConfiguration.SelectSingleNode(SR.GuardsElementName));
                this.LoadStates(workflow, workflowConfiguration.SelectSingleNode(SR.StatesElementName));    
            }
            
        }

        private readonly IDictionary<string, Type> activityTypes = new Dictionary<string, Type>();
        private readonly IDictionary<string, Type> guardTypes = new Dictionary<string, Type>();

        public void Add(XmlElement workflowConfiguration)
        {
            this.configurations.Add(workflowConfiguration);
        }

        public void AddXml(string xml) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.Add(xmlDoc.DocumentElement);
        }

        private static void LoadTypeDefinitions(XmlNode xmlNode, IDictionary<string, Type> dictionary)
        {
            if (xmlNode == null)
                return;

            foreach (var node in xmlNode.SelectNodes(SR.AddElementName).Cast<XmlElement>())
            {
                string name = node.GetAttribute("name");
                string typeName = node.GetAttribute("type");

                Type type = Type.GetType(typeName);
                if (dictionary.ContainsKey(name) && dictionary[name] != type)
                    throw new InvalidOperationException(string.Format("Activity type with name '{0}' has been already registered.", name));

                dictionary[name] = type;
            }
        }

        private void LoadActivityDefinitions(XmlNode xmlActivities)
        {
            LoadTypeDefinitions(xmlActivities, this.activityTypes);
        }

        private void LoadGuardDefinitions(XmlNode xmlGuards)
        {
            LoadTypeDefinitions(xmlGuards, this.guardTypes);
        }

        private void LoadStates<TItem>(Workflow<TItem> workflow, XmlNode xmlStates)
        {
            if (xmlStates == null)
                return;

            foreach (var xmlState in xmlStates.SelectNodes(SR.StateElementName).Cast<XmlElement>())
            {
                // state
                string stateName = xmlState.GetAttribute("name");
                workflow.AddState(stateName);

                foreach (var activity in xmlState.ChildNodes.Cast<XmlElement>())
                {
                    // activity
                    string activityTypeName = activity.Name;
                    // check fox type registration
                    if (this.activityTypes.ContainsKey(activityTypeName) == false)
#warning exception type fix
                        throw new Exception();

                    string activityName = activity.GetAttribute("name");
                    Type activityType = this.activityTypes[activityTypeName];
                    workflow.AddActivity(stateName, activityType, activity, activityName);

                    foreach (var guard in activity.ChildNodes.Cast<XmlElement>()) {
                        // guard
                        string guardTypeName = guard.Name;

                        if (this.guardTypes.ContainsKey(guardTypeName) == false)
                            throw new InvalidOperationException(string.Format("Guard type name '{0}' has not been registered.", guardTypeName));

                        workflow.AddGuard(stateName, activityType, activityName, this.guardTypes[guardTypeName], guard);
                    }
                }
            }
        }
    }
}
