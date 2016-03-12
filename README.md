#Suto Navigation

Open source library is a navigation service for UWP which leaves minimal memory footprint and support custom transitions such as Instagram like. Support for element Recycling and react to low memory.

####Basic usage

Difference transition: https://www.youtube.com/watch?v=Y3jHLENmSdo

![](https://media.giphy.com/media/yhcpSeCJHDhTi/giphy.gif)

Include the following line in your XAML page.

`xmlns:suto="using:SutoNavigation.NavigationService"`

add to the page content:

`<suto:PanelContainer/>`

or inject at run time:

    panelContainer = new PanelContainer();
    panelContainer.MinimumThresshold = 1;
    panelContainer.Navigate(typeof(ProfilePanel));
    root.Children.Add(panelContainer);

where root is a Grid or StackPanel in XAML page.

Create `UserControl` base on `PanelBase`

Navigate between Panel:

`this.Host.Navigate(typeof(ProviderPanel), null, new InstagramTransition());`

`this.Host.GoBack()`

####Create custom transition:

Create class extend from `PanelTransition`, overwrite `SetInitialState` and `CreateAnimation`, see the sample for more information.

####Use recycle mode

Set OperationMode to Recycle

`panelContainer.OperationMode = OperationMode.Recycle;`

and overwrite `ResetOnReUse` method of `PanelTransition`

####Memory management

Your application can run out of memory sometime, you can let Suto Navigation automatically clear panel in the stack for you.

Rules for built-in memory management:

- Never clear current panel
- Never clear High Importaness panel
- When memory is medium (85% of available memory pool), panels with Low Importaness will be cleared
- When memory is hight ( > 85% of available memory pool), panels with Low and Normal Importaness will be cleared

Use the following line to enable memory management:

`panelContainer.EnableAutoMemoryManagement(new BasicMemoryWatcher());`

To create custom strategy for detect low memory, implement IMemoryWatcher. Create custom implementation of IMemoryReactor.

`panelContainer.EnableAutoMemoryManagement(new BasicMemoryWatcher(), new CustomMemoryReactor());`