using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class MouseCursor : Panel
{
    public static MouseCursor Instance { get; private set; }

    public Image Cursor { get; private set; }

    public MouseCursor() : base()
    {
        StyleSheet.Load("/ui/MouseCursor.scss");

        Cursor = Add.Image( "", "cursor");
        Cursor.SetTexture("ui/crosshair/crosshair.png");

        Instance = this;
    }

    public override void Tick()
    {
        Vector2 mousePosition = Mouse.Position / Screen.Size;

        float normalX = mousePosition.x - (50 / 2 / Screen.Size.x);
        float normalY = mousePosition.y - (50 / 2 / Screen.Size.y);

        Cursor.Style.Left = Length.Fraction( normalX );
        Cursor.Style.Top = Length.Fraction( normalY );
        Cursor.Style.Dirty();

        base.Tick();
    }
}