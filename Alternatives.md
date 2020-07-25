# Alternative Approaches

As shown in the main document, the reference implementations appears to have a problem that the application
business layer has many dependencies on the framework. That goes against the recommendations of Clean Architecture
to keep any frameworks at arms length, be able to easily change them in the future, and stop them influencing 
what really matters, the business domain model. 

So what are alternatives? 

## Public Properties Manager
In [README](./README.md) so far I have proposed a very Microsoft like approach based on the fact that typically
in .net project the models are not much more than a property bag, or at least have its data exposed as public properties. In that case
the problem can be solved by a 'Manager' class that monitors and updates the public properties. While it works, and perhaps it improves the original 
design by eliminating the dependency of the business model on the framework, it requires a nor very SOLID business model. 

## IPropertyChanged notification
In this scenario a model could implement something like the `IPropertyChanged` interface and raise events whenever its property changes.
`Manager` should implement the same interface an allow the entity to subscribe to it. While it helps with dependencies, the model has to be significantly
and what worse externally changed to allow this to work

