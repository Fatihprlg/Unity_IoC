# IoC Container Example for Unity
This container architecture manages dependencies in active scene for you. You should create a SceneContext, at least a builder and other classes. Use Dependency attribute for mark dependencies to inject and use Inject() extension method on initialize to inject dependencies on startup.
I use my base class MonoBase for manage other classes. In the initializer, you can initialize MonoBase classes by given order. There is a SampleScene in this project that can help you to understand main structure. 
