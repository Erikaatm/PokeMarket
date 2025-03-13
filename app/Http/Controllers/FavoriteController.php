<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Card;
use Illuminate\Support\Facades\Auth;

class FavoriteController extends Controller
{
    public function addToFavorites($cardId)
    {
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Verificar si la carta existe
        $card = Card::find($cardId);
        if (!$card) {
            return response()->json(['message' => 'Carta no encontrada.'], 404);
        }

        // Agregar a favoritos si no está ya añadida
        if (!$user->favorites()->where('card_id', $cardId)->exists()) {
            $user->favorites()->attach($cardId);
            return response()->json(['message' => 'Carta añadida a favoritos.']);
        }

        return response()->json(['message' => 'Esta carta ya está en favoritos.'], 400);
    }

    public function removeFromFavorites($cardId)
    {
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Verificar si la carta está en favoritos antes de eliminarla
        if ($user->favorites()->where('card_id', $cardId)->exists()) {
            $user->favorites()->detach($cardId);
            return response()->json(['message' => 'Carta eliminada de favoritos.']);
        }

        return response()->json(['message' => 'Esta carta no estaba en favoritos.'], 400);
    }

    public function listFavorites()
    {
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        return response()->json($user->favorites()->get() ?? []);
    }
}