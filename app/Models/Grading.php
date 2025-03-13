<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

/**
 * 
 *
 * @property int $id
 * @property int $user_id
 * @property int $card_id
 * @property float $price
 * @property string $status
 * @property \Illuminate\Support\Carbon|null $created_at
 * @property \Illuminate\Support\Carbon|null $updated_at
 * @property-read \App\Models\Card $card
 * @property-read \App\Models\User $user
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading newModelQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading newQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading query()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading whereCardId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading whereCreatedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading whereId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading wherePrice($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading whereStatus($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading whereUpdatedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Grading whereUserId($value)
 * @mixin \Eloquent
 */
class Grading extends Model
{
    use HasFactory;

    protected $fillable = ['user_id', 'card_id', 'price', 'status'];

    public function user()
    {
        return $this->belongsTo(User::class);
    }

    public function card()
    {
        return $this->belongsTo(Card::class);
    }
}
