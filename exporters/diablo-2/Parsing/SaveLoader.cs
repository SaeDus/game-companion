using D2SSharp.Model;

public static class SaveLoader {
    public static D2Save LoadCharacter(string path) {
        byte[] bytes = File.ReadAllBytes(path);
        return D2Save.Read(bytes);
    }

    public static D2StashSave LoadStash(string path) {
        byte[] bytes = File.ReadAllBytes(path);
        return D2StashSave.Read(bytes);
    }
}