﻿using Sandbox.UI;
namespace XGUI;

public partial class Window : WorldPanel
{
	public Panel TitleBar { get; set; } = new Panel();
	public Label TitleLabel { get; set; } = new Label();
	public Panel TitleIcon { get; set; } = new Panel();
	public Panel TitleSpacer { get; set; } = new Panel();

	public Vector2 Position;
	public int ZIndex;

	public bool HasControls;
	public bool IsResizable;

	public Button ControlsClose { get; set; } = new Button();
	public Button ControlsMinimise { get; set; }
	public Button ControlsMaximise { get; set; }


	public Window()
	{
		

		// World panels are scaled down to world units,
		// so boost the panel scale to sensible default
		
		CreateTitleBar();
		AddClass( "Window" );
	}
	public void CreateTitleBar()
	{
		/*
		<style>
			Window {
				pointer-events:all;
				position: absolute;
				flex-direction:column;
				.TitleBar {
					.TitleIcon {

					}
					.TitleSpacer {
						flex-grow: 1;
						background-color: rgba(0,0,0,1);
					}
					.Control {
					}
				}
			}
		</style>
		<div class="TitleBar" @ref=TitleBar>
			<div class="TitleIcon" @ref=TitleIcon></div>
			<div>@Title</div>
			<div class="TitleSpacer" onmousedown=@DragBarDown onmouseup=@DragBarUp onmousemove=@Drag></div>
			<button class="Control" @ref=ControlsClose onclick=@Close>X</button>
		</div>
		*/
		Style.PointerEvents = PointerEvents.All;
		Style.Position = PositionMode.Absolute;
		Style.FlexDirection = FlexDirection.Column;


		TitleBar.AddClass( "TitleBar" );
		TitleIcon.AddClass( "TitleIcon" );
		TitleLabel.AddClass( "TitleLabel" );
		TitleSpacer.AddClass( "TitleSpacer" );
		ControlsClose.AddClass( "Control" );
		ControlsClose.AddClass( "CloseButton" );

		AddChild( TitleBar );
		TitleBar.AddChild( TitleIcon );
		TitleBar.AddChild( TitleLabel );
		TitleBar.AddChild( TitleSpacer );
		TitleBar.AddChild( ControlsClose );


		TitleSpacer.AddEventListener( "onmousedown", DragBarDown );
		TitleSpacer.AddEventListener( "onmouseup", DragBarUp );
		TitleSpacer.AddEventListener( "onmousedrag", Drag );
		TitleSpacer.Style.FlexGrow = 1;


		ControlsClose.AddEventListener( "onclick", Close );
		ControlsClose.Text = "X";
	}
	public void Close()
	{
		Log.Info( "close" );
		Delete();
	}

	public override void Tick()
	{
		SetChildIndex( TitleBar, 0 );
		base.Tick();
		Drag();

		if ( Style.Left == null )
		{
			Style.Left = 0;
			Style.Top = 0;
		}
		Style.Position = PositionMode.Absolute;
		Style.Left = Position.x * ScaleFromScreen;
		Style.Top = Position.y * ScaleFromScreen;
		//Style.ZIndex = Parent.ChildrenCount - Parent.GetChildIndex( this );
	}

	// -------------
	// Dragging
	// -------------
	bool Dragging = false;
	float xoff = 0;
	float yoff = 0;
	public void Drag()
	{
		if ( !Dragging ) return;
		Position.x = ((FindRootPanel().MousePosition.x) - xoff);
		Position.y = ((FindRootPanel().MousePosition.y) - yoff);
	}
	public void DragBarDown()
	{
		xoff = (float)((FindRootPanel().MousePosition.x) - Box.Rect.Left);
		yoff = (float)((FindRootPanel().MousePosition.y) - Box.Rect.Top);
		Dragging = true;
	}
	public void DragBarUp()
	{
		Dragging = false;
	}

	// -------------


	// -------------
	// Focusing
	// -------------

	protected override void OnMouseDown( MousePanelEvent e )
	{
		AcceptsFocus = true;
		if ( !HasFocus )
		{
			Focus();
		}
		//Parent.SetChildIndex( this, 0 );
		//Parent.SortChildren( x => x.HasFocus ? 1 : 0 );
		base.OnMouseDown( e );
	}
	// -------------


	public override void SetProperty( string name, string value )
	{
		switch ( name )
		{
			case "title":
				{
					TitleLabel.Text = value;
					return;
				}
		}
	}
}
