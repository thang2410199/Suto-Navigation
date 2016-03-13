#License

One line to give the program's name and a brief idea of what it does.
Copyright (C) 2016 Ngo Quoc Thang

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

You can contact me via email: thang2410199@gmail.com


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

To create custom strategy for detect and reactor to low memory, implement `IMemoryWatcher`. Create custom implementation of `IMemoryReactor`.

`panelContainer.EnableAutoMemoryManagement(new BasicMemoryWatcher(), new CustomMemoryReactor());`
