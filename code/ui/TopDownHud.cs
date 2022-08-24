using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class TopDownHud : HudEntity<RootPanel>
{
    public TopDownHud()
    {
        if ( !IsClient ) return;

        RootPanel.Style.PointerEvents = PointerEvents.None;
        RootPanel.AddChild<PointerPanel>();
        RootPanel.AddChild<MouseCursor>();
    }
}