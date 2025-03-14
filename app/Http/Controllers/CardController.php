<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Card;
use Illuminate\Support\Facades\Auth;

class CardController extends Controller
{
    // Crear nueva carta
    public function store(Request $request)
    {
        $card = Card::create([
            'user_id' => Auth::id(),
            'namePokemon' => $request->namePokemon,
            'pokemon_type' => $request->pokemon_type,
            'image' => $request->image,
            'edition' => $request->edition,
            'graded' => false,
            'grade' => null,
            'price' => $request->price,
            'is_tradeable' => $request->is_tradeable ?? false
        ]);

        return response()->json(['message' => 'Carta añadida.', 'card' => $card]);
    }

    // Ver todas las cartas disponibles
    public function index()
    {
        return response()->json(Card::with('owner')->get());
    }

    // Ver carta específica
    public function show($id)
    {
        return response()->json(Card::with('owner')->findOrFail($id));
    }

    // Actualizar carta específica
    public function update(Request $request, $id)
    {
        $card = Card::findOrFail($id);

        // Solo el propietario de la carta puede actualizarla
        if ($card->user_id !== Auth::id()) {
            return response()->json(['message' => 'No autorizado a actualizar esta carta.'], 403);
        }

        $card->update([
            'namePokemon' => $request->namePokemon ?? $card->namePokemon,
            'pokemon_type' => $request->pokemon_type ?? $card->pokemon_type,
            'image' => $request->image ?? $card->image,
            'edition' => $request->edition ?? $card->edition,
            'graded' => $request->graded ?? $card->graded,
            'grade' => $request->grade ?? $card->grade,
            'price' => $request->price ?? $card->price,
            'is_tradeable' => $request->is_tradeable ?? $card->is_tradeable,
        ]);

        return response()->json(['message' => 'Carta actualizada.', 'card' => $card]);
    }

    // Eliminar carta específica
    public function destroy($id)
    {
        $card = Card::findOrFail($id);

        // Solo el propietario de la carta puede eliminarla
        if ($card->user_id !== Auth::id()) {
            return response()->json(['message' => 'No autorizado a eliminar esta carta.'], 403);
        }

        $card->delete();

        return response()->json(['message' => 'Carta eliminada correctamente.']);
    }
}
