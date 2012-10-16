using System;

namespace TidyNet.Dom
{
	public enum NodeType : short
	{
		/// <summary> The node is an <code>Element</code>.</summary>
		ELEMENT_NODE = 1,

		/// <summary> The node is an <code>Attr</code>.</summary>
		ATTRIBUTE_NODE = 2,
		
		/// <summary> The node is a <code>Text</code> node.</summary>
		TEXT_NODE = 3,
		
		/// <summary> The node is a <code>CDATASection</code>.</summary>
		CDATA_SECTION_NODE = 4,
		
		/// <summary> The node is an <code>EntityReference</code>.</summary>
		ENTITY_REFERENCE_NODE = 5,
		
		/// <summary> The node is an <code>Entity</code>.</summary>
		ENTITY_NODE = 6,
		
		/// <summary> The node is a <code>ProcessingInstruction</code>.</summary>
		PROCESSING_INSTRUCTION_NODE = 7,
		
		/// <summary> The node is a <code>Comment</code>.</summary>
		COMMENT_NODE = 8,
		
		/// <summary> The node is a <code>Document</code>.</summary>
		DOCUMENT_NODE = 9,
		
		/// <summary> The node is a <code>DocumentType</code>.</summary>
		DOCUMENT_TYPE_NODE = 10,
		
		/// <summary> The node is a <code>DocumentFragment</code>.</summary>
		DOCUMENT_FRAGMENT_NODE = 11,
		
		/// <summary> The node is a <code>Notation</code>.</summary>
		NOTATION_NODE = 12
	}

}
