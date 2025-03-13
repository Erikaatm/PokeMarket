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

    // Ver cartas disponibles
    public function index()
    {
        return response()->json(Card::with('owner')->get());
    }

    // Ver carta específica
    public function show($id)
    {
        return response()->json(Card::with('owner')->findOrFail($id));
    }
}
