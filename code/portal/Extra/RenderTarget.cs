using Sandbox;

namespace tardis.Portals;

public class RenderTarget
{
	public Texture ColorTexture { get; set; }
	public Texture DepthTexture { get; set; }
	public RenderAttributes RenderAttributes { get; set; }
	public SceneWorld World;

	public RenderTarget( SceneWorld World, int width, int height )
	{
		this.World = World;
		Recreate( width, height );
		RenderAttributes = new();
	}
	
	public void Recreate( int width, int height )
	{
		ColorTexture = Texture.CreateRenderTarget().WithScreenFormat().WithSize( new( width, height ) ).Create();
		DepthTexture = Texture.CreateRenderTarget().WithDepthFormat().WithSize( new( width, height ) ).Create();
	}

	public void Draw( Vector3 Position, Rotation Rotation, float FOV, Rect ViewPort )
	{
		//Render.Draw.DrawScene( ColorTexture, DepthTexture, World, RenderAttributes, ViewPort, Position, Rotation, FOV );
	}

	~RenderTarget()
	{
		Dispose();
	}

	public void Dispose()
	{
		ColorTexture.Dispose();
		DepthTexture.Dispose();
	}
}
