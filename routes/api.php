<?php
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\UserController;
use App\Http\Controllers\CardController;
use App\Http\Controllers\TagController;
use App\Http\Controllers\TradeController;
use App\Http\Controllers\MessageController;
use App\Http\Controllers\GradingController;
use App\Http\Controllers\FavoriteController;

// Controlador de card
Route::apiResource('cards', CardController::class)->names([
    'index'   => 'cards.index',   // Ver cartas disponibles
    'store'   => 'cards.store',   // Crear nueva carta
    'show'    => 'cards.show',    // Ver carta específica
    'update'  => 'cards.update',  // Actualizar carta (si fuera necesario)
    'destroy' => 'cards.destroy', // Eliminar carta (si fuera necesario)
]);

// Controlador de favorite
Route::apiResource('favorites', FavoriteController::class)->names([
    'listFavorites'   => 'favorites.listFavorites',   // Obtener todas las cartas favoritas
    'addToFavorites'   => 'favorites.addToFavorites',      // Agregar carta a favoritos
    'isFavorite'    => 'favorites.isFavorite',     // Verificar si una carta está en favoritos
    'removeFromFavorites' => 'favorites.removeFromFavorites',   // Eliminar carta de favoritos
]);

// Controlador de grading
Route::apiResource('gradings', GradingController::class)->names([
    'requestGrading'   => 'gradings.requestGrading',  // Solicitar gradeo de una carta
    'approve'  => 'gradings.approve',         // Aprobar gradeo de una carta
]);




