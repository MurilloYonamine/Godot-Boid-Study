using Godot;
using System;
using System.Collections.Generic;

public class SpriteManager
{
    // === SPRITE STORAGE ===
    private List<Texture2D> _fishSprites = new List<Texture2D>();
    private Random _random = new Random();

    public void LoadSprites()
    {
        _fishSprites.Clear();
        
        const string FISHES_PATH = "res://Assets/Sprites/Fishes/Outline/";
        LoadSpritesFromDirectory(FISHES_PATH);
    }

    private void LoadSpritesFromDirectory(string directoryPath)
    {
        // Acess directory
        DirAccess directory = DirAccess.Open(directoryPath);
        
        if (directory == null)
        {
            GD.PrintErr($"Could not open directory: {directoryPath}");
            return;
        }

        // File discovery
        directory.ListDirBegin();
        string fileName = directory.GetNext();

        while (fileName != "")
        {
            if (fileName.EndsWith(".png"))
            {
                string fullPath = directoryPath + fileName;
                Texture2D texture = GD.Load<Texture2D>(fullPath);
                
                if (texture != null)
                {
                    _fishSprites.Add(texture);
                }
            }
            fileName = directory.GetNext();
        }
        
        directory.ListDirEnd();
    }

    public Texture2D GetRandomSprite()
    {
        if (_fishSprites.Count == 0)
        {
            GD.PrintErr("No sprites loaded! Call LoadSprites() first.");
            return null;
        }
        
        int index = _random.Next(_fishSprites.Count);
        return _fishSprites[index];
    }
}