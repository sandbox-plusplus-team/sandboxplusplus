// https://github.com/ValveSoftware/halflife/blob/master/dlls/combat.cpp

using Sandbox;

public partial class DWCombat
{
    [ConVar.Replicated] public static float max_gibs { get; set; } = 100;
    static public int GibCount = 0;
    static public int GibFadingCount = 0;

    public enum HoldTypes
    {
        None,
        Pistol,
        Python,
        Rifle,
        Shotgun,
        HoldItem,
        Crossbow,
        Egon,
        Gauss,
        Hive,
        RPG,
        Squeak,
        Trip,
        Punch,
        Swing
    }
    // monster to monster relationship types
    public static int R_AL = -2; // (ALLY) pals. Good alternative to R_NO when applicable.
    public static int R_FR = -1; // (FEAR)will run
    public static int R_NO = 0;  // (NO RELATIONSHIP) disregard
    public static int R_DL = 1;  // (DISLIKE) will attack
    public static int R_HT = 2;  // (HATE)will attack this character instead of any visible DISLIKEd characters
    public static int R_NM = 3;  // (NEMESIS)  A monster Will ALWAYS attack its nemsis, no matter what
	public static int R_SP = 4;  // (special)  for special interaction (most likley tardis)
	public enum Class
    {
        // For CLASSIFY
        CLASS_NONE,
        CLASS_DALEK,
        CLASS_PLAYER,
        CLASS_TARDIS,
        CLASS_HUMAN_MILITARY,
        CLASS_ALIEN_MILITARY,
        CLASS_ALIEN_PASSIVE,
        CLASS_ALIEN_MONSTER,
        CLASS_ALIEN_PREY,
        CLASS_ALIEN_PREDATOR,
        CLASS_INSECT,
        CLASS_PLAYER_ALLY,
        CLASS_PLAYER_BIOWEAPON,
        CLASS_ALIEN_BIOWEAPON,
        CLASS_BARNACLE
    }

    public static int[,] ClassMatrix = new int[14, 14]
    {			 //   NONE	 MACH	 PLYR	 HPASS	 HMIL	 AMIL	 APASS	 AMONST	APREY	 APRED	 INSECT	PLRALY	PBWPN	ABWPN
	/*NONE*/		{ R_NO  ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO,  R_NO,   R_NO,    },
	/*MACHINE*/		{ R_NO  ,R_NO   ,R_HT   ,R_SP   ,R_HT   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_DL,  R_DL,   R_DL,    },
	/*PLAYER*/		{ R_NO  ,R_DL   ,R_NO   ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO,  R_DL,   R_DL,    },
	/*HUMANPASSIVE*/{ R_NO  ,R_NO   ,R_AL   ,R_AL   ,R_HT   ,R_FR   ,R_NO   ,R_HT   ,R_DL   ,R_FR   ,R_NO   ,R_AL,  R_NO,   R_NO,    },
	/*HUMANMILITAR*/{ R_NO  ,R_NO   ,R_HT   ,R_DL   ,R_NO   ,R_HT   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_HT,  R_NO,   R_NO,    },
	/*ALIENMILITAR*/{ R_NO  ,R_DL   ,R_HT   ,R_DL   ,R_HT   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_DL,  R_NO,   R_NO,    },
	/*ALIENPASSIVE*/{ R_NO  ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO,  R_NO,   R_NO,    },
	/*ALIENMONSTER*/{ R_NO  ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_DL,  R_NO,   R_NO,    },
	/*ALIENPREY   */{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_FR   ,R_NO   ,R_DL,  R_NO,   R_NO,    },
	/*ALIENPREDATO*/{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_NO   ,R_HT   ,R_DL   ,R_NO   ,R_DL,  R_NO,   R_NO,    },
	/*INSECT*/		{ R_FR  ,R_FR   ,R_FR   ,R_FR   ,R_FR   ,R_NO   ,R_FR   ,R_FR   ,R_FR   ,R_FR   ,R_NO   ,R_FR,  R_NO,   R_NO,    },
	/*PLAYERALLY*/	{ R_NO  ,R_DL   ,R_AL   ,R_AL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO,  R_NO,   R_NO,    },
	/*PBIOWEAPON*/	{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_DL,  R_NO,   R_DL,    },
	/*ABIOWEAPON*/	{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_AL   ,R_NO   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_DL,  R_DL,   R_NO,    }
    };

 
}
