<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\User;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Hash;

class UserController extends Controller
{
    /**
     * Listar todos los usuarios (Solo para administradores).
     */
    public function listUsers()
    {
        return response()->json(User::all());
    }

    /**
     * Obtener el perfil de un usuario por ID.
     */
    public function showUsersID($id)
    {
        $user = User::findOrFail($id);
        return response()->json($user);
    }

    /**
     * Actualizar el perfil del usuario autenticado.
     */
    public function updateUser(Request $request)
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Validar los datos
        $validated = $request->validate([
            'username'  => 'sometimes|string|max:50',
            'email'     => 'sometimes|email|unique:users,email,' . $user->id,
            'phone_num' => 'sometimes|string|max:15',
            'address'   => 'sometimes|string|max:255',
            'password'  => 'sometimes|string|min:6|confirmed',
        ]);

        // Si se proporciona una nueva contraseña, la encriptamos
        if (!empty($validated['password'])) {
            $validated['password'] = Hash::make($validated['password']);
        }

        $user->update($validated);

        return response()->json([
            'message' => 'Perfil actualizado exitosamente.',
            'user'    => $user,
        ]);
    }

    /**
     * Eliminar la cuenta del usuario autenticado.
     */
    public function destroyUser()
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        $user->delete();

        return response()->json(['message' => 'Cuenta eliminada correctamente.']);
    }
}
