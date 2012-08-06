using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    internal class XmlParser
    {
        private readonly IDictionary<string, Type> guardTypes;
        private readonly IDictionary<string, Type> activityTypes;
        private readonly IDictionary<string, IList<ActivityDescription>> activityGroups;

        public XmlParser(IDictionary<string, Type> guardTypes, IDictionary<string, Type> activityTypes, IDictionary<string, IList<ActivityDescription>> activityGroups)
        {
            if (activityTypes == null)
                throw new ArgumentNullException("activityTypes");

            if (guardTypes == null)
                throw new ArgumentNullException("guardTypes");

            if (activityGroups == null)
                throw new ArgumentNullException("activityGroups");

            this.activityGroups = activityGroups;
            this.activityTypes = activityTypes;
            this.guardTypes = guardTypes;
        }

        public GuardDescription ParseGuardDescription(XmlElement xml)
        {
            // guard
            string guardTypeName = xml.Name;

            if (guardTypes.ContainsKey(guardTypeName) == false)
                throw new InvalidOperationException(string.Format("Guard type name '{0}' has not been registered.", guardTypeName));

            IDictionary<string, object> guardParameters = xml.Attributes.Cast<XmlAttribute>().ToDictionary(xa => xa.Name, xa => (object)xa.Value);
            return new GuardDescription(guardTypes[guardTypeName], guardParameters);
        }

        public IEnumerable<ActivityDescription> ParseActivityDescription(XmlElement xml)
        {
            string activityTypeName = xml.Name;

            // check fox type registration

            if (activityTypes.ContainsKey(activityTypeName))
            {
                string activityName = xml.GetAttribute("name");
                Type activityType = activityTypes[activityTypeName];
                IDictionary<string, object> parameters = xml.Attributes
                    .Cast<XmlAttribute>()
                    .Where(xa => xa.Name != "name")
                    .ToDictionary(xa => xa.Name, xa => (object)xa.Value);

                ActivityDescription activityDescription = new ActivityDescription(activityType, activityName, parameters);

                foreach (var guard in xml.ChildNodes.Cast<XmlElement>())
                {
                    // guard
                    GuardDescription guardDescription = this.ParseGuardDescription(guard);
                    activityDescription.Guards.Add(guardDescription);
                }

                return new[] { activityDescription };
            }

            if (this.activityGroups.ContainsKey(activityTypeName))
            {
                return this.activityGroups[activityTypeName];
            }

            throw new InvalidOperationException(string.Format("Activity or activity group type name '{0}' has not been registered.", activityTypeName));
        }

        public KeyValuePair<string, IList<ActivityDescription>> ParseActivityGroup(XmlElement xml)
        {
            string name = xml.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new FormatException("Activity group must have a name!");

            XmlElement activitiesNode = (XmlElement)xml.SelectSingleNode("Activities");
            List<ActivityDescription> subActivities = new List<ActivityDescription>();
            if (activitiesNode != null)
            {
                subActivities.AddRange(activitiesNode
                    .ChildNodes
                    .Cast<XmlElement>()
                    .SelectMany(ParseActivityDescription));
            }

            XmlNode guardsNode = xml.SelectSingleNode("Guards");
            List<GuardDescription> guards = new List<GuardDescription>();
            if (guardsNode != null)
            {
                guards.AddRange(guardsNode
                                    .ChildNodes
                                    .Cast<XmlElement>()
                                    .Select(ParseGuardDescription));
            }

            foreach (var activity in subActivities)
                activity.Guards.AddRange(guards);

            return new KeyValuePair<string, IList<ActivityDescription>>(name, subActivities);
        }
    }
}
