<?php
namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Grading;
use Illuminate\Support\Facades\Auth;

class GradingController extends Controller
{
    // Solicitar gradeo para una carta (equivalente a store en Laravel)
    public function store(Request $request)
    {
        // Validar los datos que nos entran
        $validated = $request->validate([
            'card_id' => 'required|exists:cards,id', // Asegúrate de que el card_id exista
            'price'   => 'required|numeric|min:0',   // Asegúrate de que price sea un número válido
        ]);

        // Crear la solicitud de gradeo
        $grading = Grading::create([
            'user_id' => Auth::id(),
            'card_id' => $validated['card_id'],
            'price'   => $validated['price'],
            'status'  => 'pending',
        ]);

        return response()->json(['message' => 'Solicitud de gradeo enviada.', 'grading' => $grading]);
    }

    // Aprobar un gradeo (equivalente a update en Laravel)
    public function update(Request $request, $id)
    {
        // Buscar el gradeo por su ID
        $grading = Grading::findOrFail($id);
        
        // Actualizar el estado a 'approved'
        $grading->update(['status' => 'approved']);

        return response()->json(['message' => 'Gradeo aprobado.']);
    }
}
