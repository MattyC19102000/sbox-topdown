using Sandbox;
using Sandbox.UI;

public partial class TopDownHud : HudEntity<RootPanel>
{
    public TopDownHud()
    {
        if ( !IsClient ) return;

        RootPanel.AddChild<MouseCursor>();
    }
}