# BlazorChat

Simple chat application based on .NET 6, Blazor WebAssembly, SignalR and Azure CosmosDB.

* Maximum size of a group is 20 members.
* Players can list, create, join and leave groups.
* Messages sent to a group are broadcast to all online players in the group.
* All messages are be stored in CosmosDB.
* When a player connects, they see message history.

Code is adapted from sample: 
[ASP.NET Core SignalR with Blazor](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr-blazor?view=aspnetcore-5.0&tabs=visual-studio&pivots=webassembly).