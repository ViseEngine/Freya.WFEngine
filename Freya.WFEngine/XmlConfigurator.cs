﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    public class XmlConfigurator
    {
        #region Fields
        private readonly IList<XmlElement> configurations = new List<XmlElement>();
        private readonly IDictionary<string, Type> activityTypes = new Dictionary<string, Type>();
        private readonly IDictionary<string, Type> guardTypes = new Dictionary<string, Type>();
        #endregion

        #region Methods
        public void Add(XmlElement workflowConfiguration) {
            this.configurations.Add(workflowConfiguration);
        }

        public void AddActivityDefinition(string activityNodeName, Type activityType) {
            AddTypeDefinition(activityNodeName, activityType, this.activityTypes);
        }

        public void AddGuardDefinition(string guardNodeName, Type guardType) {
            AddTypeDefinition(guardNodeName, guardType, this.guardTypes);
        }

        public void AddXml(string xml) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.Add(xmlDoc.DocumentElement);
        }

        public void Configure<TItem>(Workflow<TItem> workflow) {
            foreach (var workflowConfiguration in configurations) {
                this.LoadActivityDefinitions(workflowConfiguration.SelectSingleNode(SR.ActivitiesElementName));
                this.LoadGuardDefinitions(workflowConfiguration.SelectSingleNode(SR.GuardsElementName));
                this.LoadStates(workflow, workflowConfiguration.SelectSingleNode(SR.StatesElementName));    
            }
            
        }
        #endregion

        #region Helper methods
        private static void LoadTypeDefinitions(XmlNode xmlNode, IDictionary<string, Type> dictionary)
        {
            if (xmlNode == null)
                return;

            foreach (var node in xmlNode.SelectNodes(SR.AddElementName).Cast<XmlElement>())
            {
                string name = node.GetAttribute("name");
                string typeName = node.GetAttribute("type");

                Type type = Type.GetType(typeName);

                AddTypeDefinition(name, type, dictionary);
            }
        }

        private static void AddTypeDefinition(string nodeName, Type type, IDictionary<string, Type> dictionary)
        {
            if (dictionary.ContainsKey(nodeName) && dictionary[nodeName] != type)
                throw new InvalidOperationException(string.Format("Activity type with name '{0}' has been already registered.", nodeName));

            dictionary[nodeName] = type;
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
                        throw new InvalidOperationException(string.Format("Activity type name '{0}' has not been registered.", activityTypeName));

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
        #endregion
    }
}
