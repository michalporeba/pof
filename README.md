# Partially Ordered Facts

The Partially Ordered Facts or (PoF) library is an attempt at implementing the Historical Modelling idea while following principles of clean architecture, especially the need to keep any frameworks at arms length. 

## Introduction

### Historical Modelling

I have first heared about from a Pluralsight course [Occasionally Connected Windows Mobile Apps: Collaboration](https://www.pluralsight.com/courses/occasionally-connected-windows-mobile-apps-collaboration) by [Michael L. Perry](https://github.com/michaellperry) and still after 5 years since that course is a very good introduction to the concept, despite the fact that is shown on an example of Windows Phone 7 application. 

As explained by the author on [Modeling.com](https://modeling.com) Historical Modelling "is based on a model of software behavior as a graph of partially ordered facts". Unlike with distributed event sourcing system where the messages have to ordered historical modelling requires only partial ordering. 

### Original Implementations

Original implementations can be found in a number of Michael's projects [Jinaga](https://github.com/michaellperry/jinaga), [Correspondence](http://correspondencecloud.com/) and [Mathematicians](https://github.com/michaellperry/Mathematicians) which he calls a 'reference implementation'. 

### Another Approach

While the concept is very interesting and has been at the back of my mind for the last 5 years, every time I attempted to do somethinig with it very quickly I got discurrage by how invasive this approach is. It never found a way to simply add it to an existing project, without changing much more than I would want to. 

So this repository is attempt at trying to see if there is another way, one that would allow to have a library that allows to add historical modelling to any existing codebase as an extra, rather than as an architectural principle. 

## Initial Requirements

1. Entity objects should not depend on any framework.
2. It should be possible to add historical modelling like synchronisation to existing application with minimal disruption. 
3. Where possible strong typing should be chosen over dynamic / expando objects.