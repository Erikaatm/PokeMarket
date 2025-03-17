<?php
namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Tag;

class TagController extends Controller
{
    /**
     * Obtener todas las etiquetas.
     */
    public function index()
    {
        return response()->json(Tag::all());
    }

    /**
     * Crear una nueva etiqueta.
     */
    public function store(Request $request)
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
    public function show($id)
    {
        $tag = Tag::findOrFail($id);

        return response()->json($tag);
    }

    /**
     * Actualizar una etiqueta existente.
     */
    public function update(Request $request, $id)
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
    public function destroy($id)
    {
        $tag = Tag::findOrFail($id);
        $tag->delete();

        return response()->json(['message' => 'Etiqueta eliminada con éxito.']);
    }
}
