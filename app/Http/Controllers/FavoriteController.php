<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Card;
use App\Models\User;
use Illuminate\Support\Facades\Auth;
use Illuminate\Http\Response;

class FavoriteController extends Controller
{
    // Agregar una carta a favoritos
    public function addToFavorites($cardId)
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Verificar si la carta existe
        $card = Card::find($cardId);
        if (!$card) {
            return response()->json(['message' => 'Carta no encontrada.'], 404);
        }

        // Verificar si ya está en favoritos
        if ($user->favorites()->where('card_id', $cardId)->exists()) {
            return response()->json(['message' => 'Esta carta ya está en favoritos.'], 400);
        }

        // Agregar a favoritos
        $user->favorites()->attach($cardId);
        return response()->json(['message' => 'Carta añadida a favoritos.']);
    }

    // Eliminar una carta de favoritos
    public function removeFromFavorites($cardId)
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Verificar si la carta está en favoritos
        if (!$user->favorites()->where('card_id', $cardId)->exists()) {
            return response()->json(['message' => 'Esta carta no estaba en favoritos.'], 400);
        }

        // Eliminar de favoritos
        $user->favorites()->detach($cardId);
        return response()->json(['message' => 'Carta eliminada de favoritos.']);
    }

    // Obtener todas las cartas favoritas del usuario con paginación
    public function listFavorites(Request $request)
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Paginación de favoritos (por defecto 10 por página)
        $favorites = $user->favorites()->paginate($request->get('per_page', 10));

        return response()->json($favorites);
    }

    // Método para verificar si una carta está en favoritos
    public function isFavorite($cardId)
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Verificar si la carta está en favoritos
        $isFavorite = $user->favorites()->where('card_id', $cardId)->exists();

        return response()->json(['is_favorite' => $isFavorite]);
    }
}
