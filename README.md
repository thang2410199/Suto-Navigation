#Suto Navigation

Suto Navigation is a navigation service for UWP app which leaves minimal memory footprint and support custom transition. Instagram like navigation. Support for Recyle and Low Memory mode on the way.

####Basic usage

VIDEO IN ACTION: https://www.youtube.com/watch?v=Y3jHLENmSdo

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

`this.Host.Navigate(typeof(ProviderPanel), null, new InstagramPanelTransition());`

`this.Host.GoBack()`

####Create custom transition:

Create class extend from `PanelTransition`, overwrite `SetInitialState` and `CreateAnimation`, see the sample for more information
