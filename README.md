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
