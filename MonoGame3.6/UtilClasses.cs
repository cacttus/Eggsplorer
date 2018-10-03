using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Core
{

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public enum AIType
    {
        Player,
        Wandering,
        BackNForth,
    }

    public enum ArtifactType
    {
        Bone1,//0
        Bone2,//1
        Bone3,//2
        Bone4,//3
        Bone5,//4
        Bone6,//5
        Bone7,//6
        Egg1,//7
        Egg2,//8
        Pickaxe,//9
    }

    public enum GameState
    {
        Intro,
        ShowInstructions,
        LevelStart,
        LevelPlay,
        LevelEnd,
        GameOver
    }

    public class Tile : GameObject
    {
        public Tile(World w) : base(w) { }
        public DigPile Pile;
    }

    
    public class Level
    {
        public int NumCarveSteps = 100;
        public int Gridsize = 50;
        public int NumBones = 3;
        public int NumPickaxes = 1;
        public int NumBadGuys = 1;
        public ivec2 eggpos = new ivec2(0, 0);
        public string LevelSpriteName { get; private set; }
        public int Number = 0;
        public float TimeBonus = 0;

        public Level(int number, int gridsize, int steps, int bones, string spriteName, int numbadguys, float timeBon, int numPickaxes)
        {
            TimeBonus = timeBon;
            Number = number;
            Gridsize = gridsize;
            NumCarveSteps = steps;
            NumBones = bones;
            LevelSpriteName = spriteName;
            NumBadGuys = numbadguys;
            NumPickaxes = numPickaxes;
        }
    }

    public class Particle : GameObject
    {
        public Particle(World w) : base(w) { }

        public float LifeMillis = 0;
    }

    public class ParticleParams
    {
        public MpInt Count = new MpInt(10, 20);
        public MpVec2 Pos = new MpVec2(new vec2(0, 0), new vec2(0, 0));
        public MpVec2 Velocity = new MpVec2(new vec2(1, 1), new vec2(4, 4));
        public MpFloat InitialRotation = new MpFloat(0, 0);
        public MpFloat DeltaRotation = new MpFloat(0, 0);
        public MpFloat InitialFade = new MpFloat(1, 1);
        public MpFloat DeltaFade = new MpFloat(0, 0);
        public bool DieOnFade = true;
        public MpInt LifespanMillis = new MpInt(-1, -1); // -1 = forever
        public List<Sprite> PossibleSprites = new List<Sprite>();
        public List<Frame> PossibleFrames = new List<Frame>();
        public List<Color> PossibleColors = new List<Color>();
        public MpFloat InitialScale = new MpFloat(1, 1);
        public MpFloat DeltaScale = new MpFloat(0, 0);

        public ParticleParams() { }
        public ParticleParams(Frame f, MpInt count, MpVec2 pos, MpVec2 vel, MpFloat iRot, MpFloat dRot, 
            MpFloat iScale, MpFloat dScale, MpFloat iFade, MpFloat dFade, List<Color> colors)
        {
            Count = count;
            PossibleFrames.Add(f);
            Pos = pos;
            Velocity = vel;
            InitialRotation = iRot;
            DeltaRotation = dRot;
            InitialScale = iScale;
            DeltaScale = dScale;
            InitialFade = iFade;
            DeltaFade = dFade;
            PossibleColors = colors;
        }

    }

    public class Artifact : GameObject
    {
        public Artifact(World w, ArtifactType type) : base(w) {

            ArtifactType = type;

            string spriteName = "";
            if ( ArtifactType == ArtifactType.Bone1) { spriteName = (w as World).Res.SprArtifactBone1; }
            else if ( ArtifactType == ArtifactType.Bone2) { spriteName = (w as World).Res.SprArtifactBone2; }
            else if ( ArtifactType == ArtifactType.Bone3) { spriteName = (w as World).Res.SprArtifactBone3; }
            else if ( ArtifactType == ArtifactType.Bone4) { spriteName = (w as World).Res.SprArtifactBone4; }
            else if ( ArtifactType == ArtifactType.Bone5) { spriteName = (w as World).Res.SprArtifactBone5; }
            else if ( ArtifactType == ArtifactType.Bone6) { spriteName = (w as World).Res.SprArtifactBone6; }
            else if ( ArtifactType == ArtifactType.Bone7) { spriteName = (w as World).Res.SprArtifactBone7; }
            else if ( ArtifactType == ArtifactType.Egg1) { spriteName =  (w as World).Res.SprArtifactEgg1; }
            else if ( ArtifactType == ArtifactType.Egg2) { spriteName =  (w as World).Res.SprArtifactEgg2; }
            else if ( ArtifactType == ArtifactType.Pickaxe) { spriteName =  (w as World).Res.SprPickaxe; }
            else
            {
                throw new NotImplementedException();
            }
            Frame = w.Res.Tiles.GetSpriteFrame(spriteName, 0);
            SpriteName = spriteName;
        }

        public vec2 TilePos;
        public ArtifactType ArtifactType = ArtifactType.Bone1;
        public string SpriteName;
    }

    public class DigPile : GameObject
    {
        public Artifact Artifact = null;
        public ivec2 GridXY;
        public DigPile(WorldBase w, int gridx, int gridy) : base(w) { GridXY = new ivec2(gridx, gridy); }
    }
}
