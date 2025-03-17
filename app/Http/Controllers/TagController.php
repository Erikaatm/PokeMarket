<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Tag;

class TagController extends Controller
{
    /**
     * Obtener todas las etiquetas.
     */
    public function listTags()
    {
        return response()->json(Tag::all());
    }

    /**
     * Crear una nueva etiqueta.
     */
    public function addTag(Request $request)
    {
        $validated = $request->validate([
            'name' => 'required|string|unique:tags,name|max:255',
        ]);

        $tag = Tag::create($validated);

        return response()->json(['message' => 'Etiqueta creada con éxito.', 'tag' => $tag], 201);
    }

    /**
     * Obtener una etiqueta específica por ID.
     */
    public function showTagsID($id)
    {
        $tag = Tag::findOrFail($id);

        return response()->json($tag);
    }

    /**
     * Actualizar una etiqueta existente.
     */
    public function updateTag(Request $request, $id)
    {
        $tag = Tag::findOrFail($id);

        $validated = $request->validate([
            'name' => 'required|string|unique:tags,name,' . $id . '|max:255',
        ]);

        $tag->update($validated);

        return response()->json(['message' => 'Etiqueta actualizada con éxito.', 'tag' => $tag]);
    }

    /**
     * Eliminar una etiqueta.
     */
    public function destroyTag($id)
    {
        $tag = Tag::findOrFail($id);
        $tag->delete();

        return response()->json(['message' => 'Etiqueta eliminada con éxito.']);
    }
}
