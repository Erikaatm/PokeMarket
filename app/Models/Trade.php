<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

/**
 * 
 *
 * @property int $id
 * @property int $from_user_id
 * @property int $to_user_id
 * @property string $status
 * @property \Illuminate\Support\Carbon|null $created_at
 * @property \Illuminate\Support\Carbon|null $updated_at
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\Card> $cards
 * @property-read int|null $cards_count
 * @property-read \App\Models\User $fromUser
 * @property-read \App\Models\User $toUser
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade newModelQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade newQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade query()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade whereCreatedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade whereFromUserId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade whereId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade whereStatus($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade whereToUserId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Trade whereUpdatedAt($value)
 * @mixin \Eloquent
 */
class Trade extends Model
{
    use HasFactory;

    protected $fillable = ['from_user_id', 'to_user_id', 'status'];

    public function fromUser()
    {
        return $this->belongsTo(User::class, 'from_user_id');
    }

    public function toUser()
    {
        return $this->belongsTo(User::class, 'to_user_id');
    }

    public function cards()
    {
        return $this->belongsToMany(Card::class, 'trade_cards');
    }
}