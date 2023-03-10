# IoC Container for Unity

This container architecture manages dependencies in active scene for you. You should create a Context and a Builder. Use Dependency attribute for mark dependencies to inject and use Inject() extension method on initialize to inject dependencies on startup.
There is a SampleScene in this project that can help you to understand main structure. 

# Usage

A dependency-injecting container consists of a builder structure that pulls and forwards dependencies in the scene, and a Context structure that controls the builders. It has been kept very simple to use. Version v1.0 only has property and field injection. In order to use this feature, first scene setup is required. Context object and Builder object should be added to the scene and Context must be initialized before all injections via calling Initialize() method or just use the initializeOnAwake boolean for automatic initialization on Awake function.

![contHierarchy](https://user-images.githubusercontent.com/58040833/209344574-ca254685-46df-48a0-aad8-1d3a956b4b27.png)

After this step, the context pulls the builder on the scene, and the builder automatically pulls the MonoBehaviour classes on the scene every recompile. If You want to map classes manually, you can click the Map Classes On Scene button for it.

![image](https://user-images.githubusercontent.com/58040833/212704074-2ffd95aa-dba1-4e4c-92d3-e291601cc7aa.png)

After this step, you need to add the [Dependency] attribute next to the fields and properties you want to inject in the code. After this step, where you initialize the code or before using the dependency this.Inject(); Calling the extension method will suffice.

![exInject](https://user-images.githubusercontent.com/58040833/209344653-040e4833-1b7a-4233-8c6d-5c67b2ab136d.png)

If there is more than one instance of an object in a scene and you want to inject a specific one, you can use [Dependency("Name of GameObject")]. If you don't, the container will automatically inject the first object it finds.

![exInject2](https://user-images.githubusercontent.com/58040833/209344666-45b8c34f-a01e-4e4d-ab62-d3ba4d3a2225.png)
