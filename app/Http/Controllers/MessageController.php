<?php
namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Message;
use Illuminate\Support\Facades\Auth;

class MessageController extends Controller
{
    /**
     * Enviar un nuevo mensaje.
     * 
     * @param  \Illuminate\Http\Request  $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function store(Request $request)
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Validación de los datos entrantes
        $validated = $request->validate([
            'to_user_id' => 'required|exists:users,id', // Verifica que el usuario destino exista en la base de datos
            'message'    => 'required|string|max:255',  // El mensaje debe ser una cadena no vacía y con una longitud razonable
        ]);

        // Crear el mensaje en la base de datos
        $message = Message::create([
            'from_user_id' => Auth::id(),  // Obtención del ID del usuario autenticado
            'to_user_id'   => $validated['to_user_id'],
            'message'      => $validated['message'],
        ]);

        return response()->json([
            'message' => 'Mensaje enviado exitosamente.',
            'data'    => $message
        ]);
    }

    /**
     * Obtener los mensajes de un usuario autenticado.
     * 
     * @param  int  $userId
     * @param  \Illuminate\Http\Request  $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function index($userId, Request $request)
    {
        /** @var User $user */
        $user = Auth::user();

        if (!$user) {
            return response()->json(['message' => 'Usuario no autenticado.'], 401);
        }

        // Verificación de que el usuario autenticado está intentando acceder a sus propios mensajes
        if (Auth::id() !== (int)$userId) {
            return response()->json(['message' => 'Acción no autorizada.'], 403);
        }

        // Paginación de los mensajes (valor por defecto de 10 por página)
        $messages = Message::where('from_user_id', $userId)
            ->orWhere('to_user_id', $userId)
            ->paginate($request->get('per_page', 10));

        return response()->json($messages);
    }
}
