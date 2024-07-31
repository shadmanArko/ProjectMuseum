using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class DecorationServices: Node
{
    private List<DecorationOther> _decorationOtherDatabase;
    private List<DecorationOtherVariation> _decorationOtherVariationDatabase;

    public override void _Ready()
    {
        base._Ready();
    }
    //todo problem when uncomment following lines
    
    public async Task<DecorationOther> Insert(DecorationOther decorationOther)
    {
        var decorationOthers = _decorationOtherDatabase;
        decorationOthers?.Add(decorationOther);
        return decorationOther;
    }

    public Task<DecorationOther> Update(string id, DecorationOther DecorationOther)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationOther?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationOther>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<List<DecorationOtherVariation>?> GetAllDecorationOtherVariations()
    {
        var decorationOtherVariations = _decorationOtherVariationDatabase;
        return decorationOtherVariations;
    }

    public Task<DecorationOtherVariation?> GetDecorationOtherVariation(string variationName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<DecorationOther>?> GetAllDecorationOthers()
    {
        var decorationOther = _decorationOtherDatabase;
        return decorationOther;
    }

    public Task<DecorationOther?> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationOther>?> DeleteAllDecorationOthers()
    {
        throw new NotImplementedException();
    }

    public Task<DecorationOther?> AddArtifactToDecorationOther(string DecorationOtherId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationOther?> RemoveArtifactFromDecorationOther(string DecorationOtherId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }
}