**Freya Workflow Engine** (WFEngine) is a lightweight state-machine based tool for applying business processes onto business objects (_items_) for .NET applications. WFEngine targets few workflows-many items scenario, whereas most of other workflow frameworks focus on application-level control.

By using WFEngine, you can
* use any existing entity (item) for workflow state tracking
* use built-in activities or create your own activities 
* create your own activity guards (e.g. access rights, preconditions, etc.)
* build your workflows from several sources
  * XML using `XmlConfigurator` class
  * manual using `Workflow` class directly

### Basic principles
_Workflow_ describes a business process of an _item_, or rather set of _states_ and possible transitions carried by _activities_. Each item managed by a workflow is always in one of the states defined by the workflow. Each state is defined by its unique name and has a set of activities that are available for an item in the state. Availability of an activity can be further restricted by _activity guards_. Guards are predicates that determine the availability of an activity based on execution context - item, state, activity and any other factors.

#### Workflow configuration example
``` xml
<Workflow>
    <!-- Registers activity and guard types. This section can be separated to another file. -->
    <Activities>
        <Add name="Transition" type="Freya.WFEngine.TransitionActivity, Freya.WFEngine" />
    <Activities>
      
    <Guards>
        <Add name="Role" type="MyProject.RoleGuard, MyProject" />
    </Guards>

    <!-- Workflow definition -->
    <States>
        <State name="open">
            <!-- Enable transition to 'closed' for all users -->
            <Transition name="Close" exitState="closed" />
            <!-- Enable transition to 'rejected' for all users -->
            <Transition name="Reject" exitState="rejected" />
        </State>

        <State name="closed">
            <!-- Enable transition to 'rejected' for admins only -->
            <Transition name="Reject" exitState="rejected">
                <Role roles="admin" />
            </Transition>
        </State>

        <State name="rejected" />
    </States>
</Workflow>
```
To instantiate workflow, copy following code:
``` csharp
string xml = "<Workflow ..."; // add workflow configuration here
Workflow<Item> workflow = new Workflow<Item>();
XmlConfigurator configurator = new XmlConfigurator();
configurator.AddXml(xml);
configurator.Configure(workflow);

// use the workflow here
```