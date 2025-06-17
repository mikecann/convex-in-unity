import { mutation, query } from "./_generated/server";
import { v } from "convex/values";

// Query to return all documents from the documents table
export const addMessage = mutation({
  args: {
    message: v.string(),
    user: v.string(),
  },
  handler: async (ctx, args) => {
    return await ctx.db.insert("messages", args);
  },
});

export const simple = mutation({
  args: {},
  handler: async (ctx, args) => {
    return await ctx.db.insert("messages", {
      message: "simple mutation",
      user: "simple user",
    });
  },
});

export const list = query({
  args: {},
  handler: async (ctx) => {
    return await ctx.db.query("messages").collect();
  },
});
