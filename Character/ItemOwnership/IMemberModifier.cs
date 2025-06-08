using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Itemがメンバーに追加されたときに呼び出されるメソッドの引数として渡される。Itemがメンバーに影響を与えるために使用される。例えば，あるアイテムを持つメンバーにステータス効果が付与されるようなことができるようにする。
public interface IMemberModifier
{
	// ここにメンバーに影響を与えるメソッドを記述する。
}