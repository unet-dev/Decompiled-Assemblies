using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class VendingMachine : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public VendingMachine.SellOrderContainer sellOrderContainer;

		[NonSerialized]
		public string shopName;

		[NonSerialized]
		public int vmoIndex;

		public bool ShouldPool = true;

		private bool _disposed;

		public VendingMachine()
		{
		}

		public VendingMachine Copy()
		{
			VendingMachine vendingMachine = Pool.Get<VendingMachine>();
			this.CopyTo(vendingMachine);
			return vendingMachine;
		}

		public void CopyTo(VendingMachine instance)
		{
			if (this.sellOrderContainer == null)
			{
				instance.sellOrderContainer = null;
			}
			else if (instance.sellOrderContainer != null)
			{
				this.sellOrderContainer.CopyTo(instance.sellOrderContainer);
			}
			else
			{
				instance.sellOrderContainer = this.sellOrderContainer.Copy();
			}
			instance.shopName = this.shopName;
			instance.vmoIndex = this.vmoIndex;
		}

		public static VendingMachine Deserialize(Stream stream)
		{
			VendingMachine vendingMachine = Pool.Get<VendingMachine>();
			VendingMachine.Deserialize(stream, vendingMachine, false);
			return vendingMachine;
		}

		public static VendingMachine Deserialize(byte[] buffer)
		{
			VendingMachine vendingMachine = Pool.Get<VendingMachine>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				VendingMachine.Deserialize(memoryStream, vendingMachine, false);
			}
			return vendingMachine;
		}

		public static VendingMachine Deserialize(byte[] buffer, VendingMachine instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				VendingMachine.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static VendingMachine Deserialize(Stream stream, VendingMachine instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 10)
				{
					if (instance.sellOrderContainer != null)
					{
						VendingMachine.SellOrderContainer.DeserializeLengthDelimited(stream, instance.sellOrderContainer, isDelta);
					}
					else
					{
						instance.sellOrderContainer = VendingMachine.SellOrderContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 18)
				{
					instance.shopName = ProtocolParser.ReadString(stream);
				}
				else if (num == 24)
				{
					instance.vmoIndex = (int)ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			return instance;
		}

		public static VendingMachine DeserializeLength(Stream stream, int length)
		{
			VendingMachine vendingMachine = Pool.Get<VendingMachine>();
			VendingMachine.DeserializeLength(stream, length, vendingMachine, false);
			return vendingMachine;
		}

		public static VendingMachine DeserializeLength(Stream stream, int length, VendingMachine instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 10)
				{
					if (instance.sellOrderContainer != null)
					{
						VendingMachine.SellOrderContainer.DeserializeLengthDelimited(stream, instance.sellOrderContainer, isDelta);
					}
					else
					{
						instance.sellOrderContainer = VendingMachine.SellOrderContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 18)
				{
					instance.shopName = ProtocolParser.ReadString(stream);
				}
				else if (num == 24)
				{
					instance.vmoIndex = (int)ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static VendingMachine DeserializeLengthDelimited(Stream stream)
		{
			VendingMachine vendingMachine = Pool.Get<VendingMachine>();
			VendingMachine.DeserializeLengthDelimited(stream, vendingMachine, false);
			return vendingMachine;
		}

		public static VendingMachine DeserializeLengthDelimited(Stream stream, VendingMachine instance, bool isDelta)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 10)
				{
					if (instance.sellOrderContainer != null)
					{
						VendingMachine.SellOrderContainer.DeserializeLengthDelimited(stream, instance.sellOrderContainer, isDelta);
					}
					else
					{
						instance.sellOrderContainer = VendingMachine.SellOrderContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 18)
				{
					instance.shopName = ProtocolParser.ReadString(stream);
				}
				else if (num == 24)
				{
					instance.vmoIndex = (int)ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public virtual void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			this.ResetToPool();
			this._disposed = true;
		}

		public virtual void EnterPool()
		{
			this._disposed = true;
		}

		public void FromProto(Stream stream, bool isDelta = false)
		{
			VendingMachine.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			VendingMachine.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(VendingMachine instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.sellOrderContainer != null)
			{
				instance.sellOrderContainer.ResetToPool();
				instance.sellOrderContainer = null;
			}
			instance.shopName = string.Empty;
			instance.vmoIndex = 0;
			Pool.Free<VendingMachine>(ref instance);
		}

		public void ResetToPool()
		{
			VendingMachine.ResetToPool(this);
		}

		public static void Serialize(Stream stream, VendingMachine instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.sellOrderContainer != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				VendingMachine.SellOrderContainer.Serialize(memoryStream, instance.sellOrderContainer);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.shopName != null)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.shopName);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.vmoIndex);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, VendingMachine instance, VendingMachine previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.sellOrderContainer != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				VendingMachine.SellOrderContainer.SerializeDelta(memoryStream, instance.sellOrderContainer, previous.sellOrderContainer);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.shopName != null && instance.shopName != previous.shopName)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.shopName);
			}
			if (instance.vmoIndex != previous.vmoIndex)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.vmoIndex);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, VendingMachine instance)
		{
			byte[] bytes = VendingMachine.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(VendingMachine instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				VendingMachine.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			VendingMachine.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return VendingMachine.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			VendingMachine.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, VendingMachine previous)
		{
			if (previous == null)
			{
				VendingMachine.Serialize(stream, this);
				return;
			}
			VendingMachine.SerializeDelta(stream, this, previous);
		}

		public class SellOrder : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public int itemToSellID;

			[NonSerialized]
			public int itemToSellAmount;

			[NonSerialized]
			public int currencyID;

			[NonSerialized]
			public int currencyAmountPerItem;

			[NonSerialized]
			public int inStock;

			[NonSerialized]
			public bool currencyIsBP;

			[NonSerialized]
			public bool itemToSellIsBP;

			public bool ShouldPool;

			private bool _disposed;

			public SellOrder()
			{
			}

			public VendingMachine.SellOrder Copy()
			{
				VendingMachine.SellOrder sellOrder = Pool.Get<VendingMachine.SellOrder>();
				this.CopyTo(sellOrder);
				return sellOrder;
			}

			public void CopyTo(VendingMachine.SellOrder instance)
			{
				instance.itemToSellID = this.itemToSellID;
				instance.itemToSellAmount = this.itemToSellAmount;
				instance.currencyID = this.currencyID;
				instance.currencyAmountPerItem = this.currencyAmountPerItem;
				instance.inStock = this.inStock;
				instance.currencyIsBP = this.currencyIsBP;
				instance.itemToSellIsBP = this.itemToSellIsBP;
			}

			public static VendingMachine.SellOrder Deserialize(Stream stream)
			{
				VendingMachine.SellOrder sellOrder = Pool.Get<VendingMachine.SellOrder>();
				VendingMachine.SellOrder.Deserialize(stream, sellOrder, false);
				return sellOrder;
			}

			public static VendingMachine.SellOrder Deserialize(byte[] buffer)
			{
				VendingMachine.SellOrder sellOrder = Pool.Get<VendingMachine.SellOrder>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					VendingMachine.SellOrder.Deserialize(memoryStream, sellOrder, false);
				}
				return sellOrder;
			}

			public static VendingMachine.SellOrder Deserialize(byte[] buffer, VendingMachine.SellOrder instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					VendingMachine.SellOrder.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static VendingMachine.SellOrder Deserialize(Stream stream, VendingMachine.SellOrder instance, bool isDelta)
			{
				while (true)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						break;
					}
					if (num <= 24)
					{
						if (num == 8)
						{
							instance.itemToSellID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.itemToSellAmount = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.currencyID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num <= 40)
					{
						if (num == 32)
						{
							instance.currencyAmountPerItem = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 40)
						{
							instance.inStock = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 48)
					{
						instance.currencyIsBP = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.itemToSellIsBP = ProtocolParser.ReadBool(stream);
						continue;
					}
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				return instance;
			}

			public static VendingMachine.SellOrder DeserializeLength(Stream stream, int length)
			{
				VendingMachine.SellOrder sellOrder = Pool.Get<VendingMachine.SellOrder>();
				VendingMachine.SellOrder.DeserializeLength(stream, length, sellOrder, false);
				return sellOrder;
			}

			public static VendingMachine.SellOrder DeserializeLength(Stream stream, int length, VendingMachine.SellOrder instance, bool isDelta)
			{
				long position = stream.Position + (long)length;
				while (stream.Position < position)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						throw new EndOfStreamException();
					}
					if (num <= 24)
					{
						if (num == 8)
						{
							instance.itemToSellID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.itemToSellAmount = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.currencyID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num <= 40)
					{
						if (num == 32)
						{
							instance.currencyAmountPerItem = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 40)
						{
							instance.inStock = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 48)
					{
						instance.currencyIsBP = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.itemToSellIsBP = ProtocolParser.ReadBool(stream);
						continue;
					}
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				if (stream.Position != position)
				{
					throw new ProtocolBufferException("Read past max limit");
				}
				return instance;
			}

			public static VendingMachine.SellOrder DeserializeLengthDelimited(Stream stream)
			{
				VendingMachine.SellOrder sellOrder = Pool.Get<VendingMachine.SellOrder>();
				VendingMachine.SellOrder.DeserializeLengthDelimited(stream, sellOrder, false);
				return sellOrder;
			}

			public static VendingMachine.SellOrder DeserializeLengthDelimited(Stream stream, VendingMachine.SellOrder instance, bool isDelta)
			{
				long position = (long)ProtocolParser.ReadUInt32(stream);
				position += stream.Position;
				while (stream.Position < position)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						throw new EndOfStreamException();
					}
					if (num <= 24)
					{
						if (num == 8)
						{
							instance.itemToSellID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.itemToSellAmount = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.currencyID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num <= 40)
					{
						if (num == 32)
						{
							instance.currencyAmountPerItem = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 40)
						{
							instance.inStock = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 48)
					{
						instance.currencyIsBP = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.itemToSellIsBP = ProtocolParser.ReadBool(stream);
						continue;
					}
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				if (stream.Position != position)
				{
					throw new ProtocolBufferException("Read past max limit");
				}
				return instance;
			}

			public virtual void Dispose()
			{
				if (this._disposed)
				{
					return;
				}
				this.ResetToPool();
				this._disposed = true;
			}

			public virtual void EnterPool()
			{
				this._disposed = true;
			}

			public void FromProto(Stream stream, bool isDelta = false)
			{
				VendingMachine.SellOrder.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				VendingMachine.SellOrder.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(VendingMachine.SellOrder instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.itemToSellID = 0;
				instance.itemToSellAmount = 0;
				instance.currencyID = 0;
				instance.currencyAmountPerItem = 0;
				instance.inStock = 0;
				instance.currencyIsBP = false;
				instance.itemToSellIsBP = false;
				Pool.Free<VendingMachine.SellOrder>(ref instance);
			}

			public void ResetToPool()
			{
				VendingMachine.SellOrder.ResetToPool(this);
			}

			public static void Serialize(Stream stream, VendingMachine.SellOrder instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.itemToSellID);
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.itemToSellAmount);
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.currencyID);
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.currencyAmountPerItem);
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.inStock);
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.currencyIsBP);
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.itemToSellIsBP);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, VendingMachine.SellOrder instance, VendingMachine.SellOrder previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.itemToSellID != previous.itemToSellID)
				{
					stream.WriteByte(8);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.itemToSellID);
				}
				if (instance.itemToSellAmount != previous.itemToSellAmount)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.itemToSellAmount);
				}
				if (instance.currencyID != previous.currencyID)
				{
					stream.WriteByte(24);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.currencyID);
				}
				if (instance.currencyAmountPerItem != previous.currencyAmountPerItem)
				{
					stream.WriteByte(32);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.currencyAmountPerItem);
				}
				if (instance.inStock != previous.inStock)
				{
					stream.WriteByte(40);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.inStock);
				}
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.currencyIsBP);
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.itemToSellIsBP);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, VendingMachine.SellOrder instance)
			{
				byte[] bytes = VendingMachine.SellOrder.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(VendingMachine.SellOrder instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					VendingMachine.SellOrder.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				VendingMachine.SellOrder.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return VendingMachine.SellOrder.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				VendingMachine.SellOrder.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, VendingMachine.SellOrder previous)
			{
				if (previous == null)
				{
					VendingMachine.SellOrder.Serialize(stream, this);
					return;
				}
				VendingMachine.SellOrder.SerializeDelta(stream, this, previous);
			}
		}

		public class SellOrderContainer : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public List<VendingMachine.SellOrder> sellOrders;

			public bool ShouldPool;

			private bool _disposed;

			public SellOrderContainer()
			{
			}

			public VendingMachine.SellOrderContainer Copy()
			{
				VendingMachine.SellOrderContainer sellOrderContainer = Pool.Get<VendingMachine.SellOrderContainer>();
				this.CopyTo(sellOrderContainer);
				return sellOrderContainer;
			}

			public void CopyTo(VendingMachine.SellOrderContainer instance)
			{
				throw new NotImplementedException();
			}

			public static VendingMachine.SellOrderContainer Deserialize(Stream stream)
			{
				VendingMachine.SellOrderContainer sellOrderContainer = Pool.Get<VendingMachine.SellOrderContainer>();
				VendingMachine.SellOrderContainer.Deserialize(stream, sellOrderContainer, false);
				return sellOrderContainer;
			}

			public static VendingMachine.SellOrderContainer Deserialize(byte[] buffer)
			{
				VendingMachine.SellOrderContainer sellOrderContainer = Pool.Get<VendingMachine.SellOrderContainer>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					VendingMachine.SellOrderContainer.Deserialize(memoryStream, sellOrderContainer, false);
				}
				return sellOrderContainer;
			}

			public static VendingMachine.SellOrderContainer Deserialize(byte[] buffer, VendingMachine.SellOrderContainer instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					VendingMachine.SellOrderContainer.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static VendingMachine.SellOrderContainer Deserialize(Stream stream, VendingMachine.SellOrderContainer instance, bool isDelta)
			{
				if (!isDelta && instance.sellOrders == null)
				{
					instance.sellOrders = Pool.Get<List<VendingMachine.SellOrder>>();
				}
				while (true)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						break;
					}
					if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else
					{
						instance.sellOrders.Add(VendingMachine.SellOrder.DeserializeLengthDelimited(stream));
					}
				}
				return instance;
			}

			public static VendingMachine.SellOrderContainer DeserializeLength(Stream stream, int length)
			{
				VendingMachine.SellOrderContainer sellOrderContainer = Pool.Get<VendingMachine.SellOrderContainer>();
				VendingMachine.SellOrderContainer.DeserializeLength(stream, length, sellOrderContainer, false);
				return sellOrderContainer;
			}

			public static VendingMachine.SellOrderContainer DeserializeLength(Stream stream, int length, VendingMachine.SellOrderContainer instance, bool isDelta)
			{
				if (!isDelta && instance.sellOrders == null)
				{
					instance.sellOrders = Pool.Get<List<VendingMachine.SellOrder>>();
				}
				long position = stream.Position + (long)length;
				while (stream.Position < position)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						throw new EndOfStreamException();
					}
					if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else
					{
						instance.sellOrders.Add(VendingMachine.SellOrder.DeserializeLengthDelimited(stream));
					}
				}
				if (stream.Position != position)
				{
					throw new ProtocolBufferException("Read past max limit");
				}
				return instance;
			}

			public static VendingMachine.SellOrderContainer DeserializeLengthDelimited(Stream stream)
			{
				VendingMachine.SellOrderContainer sellOrderContainer = Pool.Get<VendingMachine.SellOrderContainer>();
				VendingMachine.SellOrderContainer.DeserializeLengthDelimited(stream, sellOrderContainer, false);
				return sellOrderContainer;
			}

			public static VendingMachine.SellOrderContainer DeserializeLengthDelimited(Stream stream, VendingMachine.SellOrderContainer instance, bool isDelta)
			{
				if (!isDelta && instance.sellOrders == null)
				{
					instance.sellOrders = Pool.Get<List<VendingMachine.SellOrder>>();
				}
				long position = (long)ProtocolParser.ReadUInt32(stream);
				position += stream.Position;
				while (stream.Position < position)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						throw new EndOfStreamException();
					}
					if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else
					{
						instance.sellOrders.Add(VendingMachine.SellOrder.DeserializeLengthDelimited(stream));
					}
				}
				if (stream.Position != position)
				{
					throw new ProtocolBufferException("Read past max limit");
				}
				return instance;
			}

			public virtual void Dispose()
			{
				if (this._disposed)
				{
					return;
				}
				this.ResetToPool();
				this._disposed = true;
			}

			public virtual void EnterPool()
			{
				this._disposed = true;
			}

			public void FromProto(Stream stream, bool isDelta = false)
			{
				VendingMachine.SellOrderContainer.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				VendingMachine.SellOrderContainer.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(VendingMachine.SellOrderContainer instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				if (instance.sellOrders != null)
				{
					for (int i = 0; i < instance.sellOrders.Count; i++)
					{
						if (instance.sellOrders[i] != null)
						{
							instance.sellOrders[i].ResetToPool();
							instance.sellOrders[i] = null;
						}
					}
					List<VendingMachine.SellOrder> sellOrders = instance.sellOrders;
					Pool.FreeList<VendingMachine.SellOrder>(ref sellOrders);
					instance.sellOrders = sellOrders;
				}
				Pool.Free<VendingMachine.SellOrderContainer>(ref instance);
			}

			public void ResetToPool()
			{
				VendingMachine.SellOrderContainer.ResetToPool(this);
			}

			public static void Serialize(Stream stream, VendingMachine.SellOrderContainer instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.sellOrders != null)
				{
					for (int i = 0; i < instance.sellOrders.Count; i++)
					{
						VendingMachine.SellOrder item = instance.sellOrders[i];
						stream.WriteByte(10);
						memoryStream.SetLength((long)0);
						VendingMachine.SellOrder.Serialize(memoryStream, item);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, VendingMachine.SellOrderContainer instance, VendingMachine.SellOrderContainer previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.sellOrders != null)
				{
					for (int i = 0; i < instance.sellOrders.Count; i++)
					{
						VendingMachine.SellOrder item = instance.sellOrders[i];
						stream.WriteByte(10);
						memoryStream.SetLength((long)0);
						VendingMachine.SellOrder.SerializeDelta(memoryStream, item, item);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, VendingMachine.SellOrderContainer instance)
			{
				byte[] bytes = VendingMachine.SellOrderContainer.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(VendingMachine.SellOrderContainer instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					VendingMachine.SellOrderContainer.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				VendingMachine.SellOrderContainer.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return VendingMachine.SellOrderContainer.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				VendingMachine.SellOrderContainer.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, VendingMachine.SellOrderContainer previous)
			{
				if (previous == null)
				{
					VendingMachine.SellOrderContainer.Serialize(stream, this);
					return;
				}
				VendingMachine.SellOrderContainer.SerializeDelta(stream, this, previous);
			}
		}
	}
}