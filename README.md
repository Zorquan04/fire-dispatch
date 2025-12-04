# FireDispatch – Fire Department Dispatch Simulator

This project presents a simple call management system for fire departments.

Emergency incidents appear on the map, and the app selects and dispatches the nearest available vehicles. Every stage of the operation is logged – from the call, through departure, arrival, and response, to return to the station. The system also recognizes the possibility of false alarms.

## 1. Features

- Generate random events (Pz, Mz, Af)
- Assign event identifiers (e.g., Mz-12)
- Dispatch the appropriate number of vehicles to a report
- Log unit actions (departure → arrival → actions → return)
- Handling false alarms (5% chance, quick return)
- Event queue when there are no available vehicles
- Statistics after the simulation is complete
- Design patterns: Strategy, Iterator, Observer
- Program runs asynchronously

# 2. Technologies and Concepts

- C# / .NET entire project
- Object-oriented programming broken down into models and logic
- Observer notifies loggers about events
- Strategy selects vehicle dispatch method
- Iterator browses vehicle collections
- Task/async await simulation of action and arrival times

# 3. How run
```bash
git clone <https://github.com/Zorquan04/fire-dispatch.git>
cd FireDispatchSolution/FireDispatch.App
dotnet run
```
After starting, simulation logs will appear in the console.

# Sample log
```bash
[LOG] --- NOWE ZDARZENIE: Pz-1 ---
[LOG] Lokalizacja: 50,04267, 19,93776
[LOG] [JRG-1] Pojazd JRG1-V1 przypisany do zdarzenia Pz-1
[LOG] [JRG-1] Pojazd JRG1-V2 przypisany do zdarzenia Pz-1
[LOG] [JRG-1] Pojazd JRG1-V3 przypisany do zdarzenia Pz-1
[LOG] [JRG-1] Pojazd JRG1-V1 w drodze do zdarzenia Pz-1
[LOG] Czas dojazdu pojazdu JRG1-V1: 2,9s
...
```