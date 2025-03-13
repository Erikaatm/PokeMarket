<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Grading;
use Illuminate\Support\Facades\Auth;

class GradingController extends Controller
{
    public function requestGrading(Request $request)
    {
        $grading = Grading::create([
            'user_id' => Auth::id(),
            'card_id' => $request->card_id,
            'price' => $request->price,
            'status' => 'pending'
        ]);

        return response()->json(['message' => 'Solicitud de gradeo enviada.', 'grading' => $grading]);
    }

    public function approve($id)
    {
        $grading = Grading::findOrFail($id);
        $grading->update(['status' => 'approved']);
        return response()->json(['message' => 'Gradeo aprobado.']);
    }
}
