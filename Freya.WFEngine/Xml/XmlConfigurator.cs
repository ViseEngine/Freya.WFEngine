#region License
// 
// Author: Lukas Paluzga <sajagi@freya.cz>
// Copyright (c) 2012, Lukas Paluzga
//  
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//  
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    public class XmlConfigurator
    {
        private readonly XmlParser parser;

        #region Fields
        private readonly IList<XmlElement> configurations = new List<XmlElement>();
        private readonly IDictionary<string, Type> activityTypes = new Dictionary<string, Type>();
        private readonly IDictionary<string, Type> guardTypes = new Dictionary<string, Type>();
        private readonly IDictionary<string, IList<ActivityDescription>> activityGroups = new Dictionary<string, IList<ActivityDescription>>(); 
        #endregion

        public XmlConfigurator() {
            parser = new XmlParser(this.guardTypes, this.activityTypes, this.activityGroups);
        }
           

        #region Methods
        public void Add(XmlElement workflowConfiguration) {
            this.configurations.Add(workflowConfiguration);
        }

        /// <summary>
        /// Adds activity type definition.
        /// </summary>
        /// <param name="activityNodeName">name of the xml element.</param>
        /// <param name="activityType">activity type</param>
        public void AddActivityDefinition(string activityNodeName, Type activityType) {
            AddTypeDefinition(activityNodeName, activityType, this.activityTypes);
        }

        /// <summary>
        /// Adds activity guard type definition.
        /// </summary>
        /// <param name="guardNodeName">name of the xml element</param>
        /// <param name="guardType">activity guard type</param>
        public void AddGuardDefinition(string guardNodeName, Type guardType) {
            AddTypeDefinition(guardNodeName, guardType, this.guardTypes);
        }

        /// <summary>
        /// Adds workflow configuration from xml string.
        /// </summary>
        public void AddXml(string xml) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.Add(xmlDoc.DocumentElement);
        }

        /// <summary>
        /// Updates the specified workflow according to configuration.
        /// </summary>
        public void Configure<TItem>(Workflow<TItem> workflow) {
            foreach (var workflowConfiguration in configurations) {
                this.LoadActivityDefinitions(workflowConfiguration.SelectSingleNode(SR.ActivitiesElementName));
                this.LoadGuardDefinitions(workflowConfiguration.SelectSingleNode(SR.GuardsElementName));
                this.LoadActivityGroups(workflowConfiguration.SelectSingleNode(SR.ActivityGroupsElementName));
                this.LoadStates(workflow, workflowConfiguration.SelectSingleNode(SR.StatesElementName));    
            }
        }

      

        #endregion

        #region Helper methods
        private void LoadActivityGroups(XmlNode selectSingleNode) {
            if (selectSingleNode == null)
                return;
            
            foreach (var xml in selectSingleNode.ChildNodes.Cast<XmlElement>()) {
                var group = this.parser.ParseActivityGroup(xml);
                this.activityGroups.Add(group);
            }
        }

        private static void LoadTypeDefinitions(XmlNode xmlNode, IDictionary<string, Type> dictionary)
        {
            if (xmlNode == null)
                return;

            foreach (var node in xmlNode.SelectNodes(SR.AddElementName).Cast<XmlElement>())
            {
                string name = node.GetAttribute("name");
                string typeName = node.GetAttribute("type");

                Type type = Type.GetType(typeName, true);

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
                State state = workflow.States.Add(stateName);

                foreach (var activity in xmlState.ChildNodes.Cast<XmlElement>()) {
                    IEnumerable<ActivityDescription> activityDescriptions = this.parser.ParseActivityDescription(activity);
                    foreach (var activityDescription in activityDescriptions)
                        state.Activities.Add(activityDescription);
                }
            }
        }
        #endregion
    }
}
