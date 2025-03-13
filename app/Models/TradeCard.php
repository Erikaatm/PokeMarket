<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

/**
 * 
 *
 * @property int $id
 * @property int $trade_id
 * @property int $card_id
 * @method static \Illuminate\Database\Eloquent\Builder<static>|TradeCard newModelQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|TradeCard newQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|TradeCard query()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|TradeCard whereCardId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|TradeCard whereId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|TradeCard whereTradeId($value)
 * @mixin \Eloquent
 */
class TradeCard extends Model
{
    use HasFactory;

    protected $fillable = ['trade_id', 'card_id'];
}