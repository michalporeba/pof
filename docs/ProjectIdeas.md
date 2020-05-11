# Project Ideas 

A list of simple project ideas that that could use PoF library to allow online collaboration. This collection should help guide decisions about feature design and development, but also help present the sort of problems that could be solved with the proposed approach. 

## Assessments

Having multiple people express opinion on a particular subject to assess risk, valuation, condition. It could be done in an _off-line_ mode, where there is no synchronisation between the individual devices, and then when synchronisation is enabled, in an _on-line_ mode the individual versions of a single model would be automatically merged potentially with some automatic conflict resolution by some sort of voting, or manual user decision if the automatic resolution is not possible.

## Inventory / Stock management

A stock inventory presents a number of challanges. Frequently they are taken in places with intermittent network connectivity, and potentially by multiple people at the same time. There is some potential for automated conflict resolution with things being counted mulitple times, perhaps with different values. See the [detail description](./DemoProject.md) of the demo problem and projcet. 

## Shopping Basket

A shared session between multiple people sharing a single shopping backet. Anybody can customise the order in real time. 

## Task board 

This was implemented in the Pluralsight course as the demo. A simply kanban like board that allows to create tasks and move them between a few different columns indicating their state. The cards can be managed on any of the connected devices regardless if they are on or off-line. It can lead to conflicts as the card can be moved in different directions, or edited concurrently. The conflicts then are identified and the user can resolve them by choosing one from the candidate states. 

## Vending Machines

If a set of vending machines has to manage shared inventory when not always connected to wider network.