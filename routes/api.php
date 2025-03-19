<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\CardController;
use App\Http\Controllers\FavoriteController;
use App\Http\Controllers\GradingController;
use App\Http\Controllers\MessageController;
use App\Http\Controllers\TagController;
use App\Http\Controllers\TradeController;
use App\Http\Controllers\UserController;
use App\Http\Controllers\AuthController;


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
    'index' => 'favorites.index',   // Obtener todas las cartas favoritas
    'store' => 'favorites.store',  // Agregar carta a favoritos
    'show' => 'favorites.show',       // Verificar si una carta está en favoritos
    'destroy' => 'favorites.destroy', // Eliminar carta de favoritos
]);

// Controlador de grading
Route::apiResource('gradings', GradingController::class)->names([
    'store' => 'gradings.store',  // Solicitar gradeo de una carta
    'update' => 'gradings.update', // Aprobar gradeo de una carta
]);

// Controlador de mensaje
Route::apiResource('messages', MessageController::class)->names([
    'store' => 'messages.store',      // Enviar un nuevo mensaje
    'index' => 'messages.index',     // Obtener los mensajes de un usuario
]);

// Controlador de Tags
Route::apiResource('tags', TagController::class)->names([
    'index' => 'tags.index',    // Obtener todas las etiquetas
    'store' => 'tags.store',      // Crear una nueva etiqueta
    'show'  => 'tags.show',  // Obtener una etiqueta por ID
    'update' => 'tags.update',  // Actualizar una etiqueta
    'destroy' => 'tags.destroy', // Eliminar una etiqueta
]);

// Controlador de Trades
Route::apiResource('trades', TradeController::class)->names([
    'store' => 'trades.store',    // Crear una solicitud de intercambio
    'update' => 'trades.update', // Aceptar un intercambio
    'destroy' => 'trades.destroy', // Rechazar un intercambio
]);

// Controlador de Users
Route::apiResource('users', UserController::class)->names([
    'index' => 'users.index',     // Listar todos los usuarios (solo para administradores)
    'show'  => 'users.show',   // Ver un usuario por ID
    'update' => 'users.update',   // Actualizar perfil
    'destroy' => 'users.destroy', // Eliminar cuenta
]);

// Autenticación

// Login
Route::post('/login', [AuthController::class, 'login'])->name('auth.login');

// Register
Route::post('/register', [AuthController::class, 'register'])->name('auth.register');

// Logout, solo se hace si el usuario esta autenticado
Route::middleware(['auth:sanctum'])->group(function () {
    Route::post('/logout', [AuthController::class, 'logout'])->name('auth.logout');
});
