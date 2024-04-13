using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

public abstract class Document
{
    private static int _id;
    private string _uniqueIdentifier = GenerateCharacterId();

    public DocumentOptions Options = new DocumentOptions();

    public string UniqueIdentifier => _uniqueIdentifier;

    public string Name = GenerateName();

    public DocumentTypes DocumentType;

    public virtual void OnShow(ImGuiRenderer renderer)
    {

    }

    public virtual void OnHide(ImGuiRenderer renderer)
    {

    }

    public abstract void Create();

    public abstract void BuildUIEdit(ImGuiRenderer renderer, bool readOnly);

    public abstract void BuildUINew(ImGuiRenderer renderer);

    public abstract void BuildUIDocument(ImGuiRenderer renderer);

    public abstract IEnumerable<IFileHandler> GetLoadHandlers();
    public abstract IEnumerable<IFileHandler> GetSaveHandlers();

    public abstract bool HydrateFromFileHandler(IFileHandler handler, string file);

    public abstract object DehydrateToFileHandler(IFileHandler handler, string file);

    private static string GenerateName() =>
        $"Document|{GenerateCharacterId()}";

    protected static string GenerateCharacterId()
    {
        char[] characters = new char[6];
        foreach (var index in Enumerable.Range(1, 6))
        {
            characters[index - 1] = (char)Random.Shared.Next((int)'a', ((int)'z') + 1);
        }
        return new string(characters);
    }

}
