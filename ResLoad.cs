using UnityEngine;
using System.Collections;

public static class ResLoad  {
	public static Texture camera_change_icon=Resources.Load<Texture>("camera_change_icon");
	public static Texture indicator_on_tx=Resources.Load<Texture>("indicator_on");
	public static Texture indicator_off_tx=Resources.Load<Texture>("indicator_off");
	public static Texture toLeft_tx=Resources.Load<Texture>("toLeft_tx");
	public static Texture toLeft_a_tx=Resources.Load<Texture>("toLeft_a_tx");
	public static Texture toRight_tx=Resources.Load<Texture>("toRight_tx");
	public static Texture toRight_a_tx=Resources.Load<Texture>("toRight_a_tx");

	public static Texture selection_frame_tx=Resources.Load<Texture>("selected_frame");
	public static Texture default_tx=Resources.Load<Texture>("default_button_tx");

	public static LineRenderer laser_ray;
	public static GameObject fire;
	public static GameObject pieces;
	public static GameObject pieces_s;
	public static GameObject laser_splash_sprite;
	public static GameObject warming_splash_sprite;
	public static GameObject simple_wreck;
	public static GameObject small_explosion;
	public static GameObject lf_explosion;
	public static GameObject light_splash;
	public static GameObject torpedo;
	public static GameObject explosion;
	public static GameObject nullPref;
}
