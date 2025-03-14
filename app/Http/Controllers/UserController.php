<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\User;
use Illuminate\Support\Facades\Auth;
use Illuminate\Http\Response;



class UserController extends Controller
{
    // Obtener perfil de un usuario
    public function show($id)
    {
        return response()->json(User::findOrFail($id));
    }

    // Editar datos del usuario autenticado
    public function update(Request $request)
    {
        /** @var User $user */
        $user = Auth::user(); // Asegurar que es un User
        $user->update($request->only(['username', 'email', 'phone_num', 'address']));
    
        return response()->json(['message' => 'Perfil actualizado.', 'user' => $user]);
    }
    
}
