using UnityEngine;
using System.Collections;

/**
    This class contains the global variables (states) of the various types of objects or scenes the player is interacting with as this affects the sounds to be played.
*/
public static class Globals
{
    // Walking surface (describing Isaura's current walking surface)
    public enum WalkingSurface
    {
        GRASS = 0,
        WOOD,
        DIRT,
		STONE,
        NONE
    };
    public static WalkingSurface CURRENT_SURFACE = WalkingSurface.NONE;

    // Ambiance (describing the game's current scene)
    public enum Ambiance
    {
        JUNGLE = 0,
        PLANTATION,
        CAVE,
        SENZALA,
        DUNGEON,
        MASTERS_HOUSE, 
        SLAVE_SHIP,
        HEAVENWARD_TIDES,
        NONE,

        DEBUG
    };
    public static Ambiance CURRENT_AMBIANCE = Ambiance.NONE;

    // Climbing surface (describing Isaura's current climbing object)
    public enum ClimbingSurface
    {
        ROPE = 0,
        CHAIN,
        NONE
    };
    public static ClimbingSurface CURRENT_ROPE = ClimbingSurface.NONE;

    // Cart surface (describing the surface that the draggable cart is on)
    public enum CartSurface
    {
        DIRT = 0,
        GRASS,
        STONE,
        WOOD,
        NONE
    };
    public static CartSurface CURRENT_CART_SURFACE = CartSurface.NONE;
}
